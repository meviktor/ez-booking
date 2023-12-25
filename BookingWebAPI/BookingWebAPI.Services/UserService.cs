using BookingWebAPI.Common.Constants;
using BookingWebAPI.Common.Enums;
using BookingWebAPI.Common.ErrorCodes;
using BookingWebAPI.Common.Exceptions;
using BookingWebAPI.Common.Models;
using BookingWebAPI.Common.Models.Config;
using BookingWebAPI.Common.Utils;
using BookingWebAPI.DAL.Interfaces;
using BookingWebAPI.Services.Interfaces;
using Hangfire;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Options;
using System.Text.RegularExpressions;

namespace BookingWebAPI.Services
{
    internal class UserService : IUserService
    {
        private readonly IOptions<JwtConfiguration> _jwtConfiguration;
        private readonly IUserRepository _userRepository;
        private readonly ISettingService _settingService;
        private readonly IBackgroundJobClient _jobClient;
        private readonly ISiteRepository _siteRepository;

        public UserService(IOptions<JwtConfiguration> jwtConfiguration, IUserRepository userRepository, ISettingService settingService, IBackgroundJobClient jobClient, ISiteRepository siteRepository)
        {
            _jwtConfiguration = jwtConfiguration;
            _userRepository = userRepository;
            _settingService = settingService;
            _jobClient = jobClient;
            _siteRepository = siteRepository;
        }

        public async Task<BookingWebAPIUser?> GetAsync(Guid id) => await _userRepository.GetAsync(id);

        public async Task<BookingWebAPIUser> RegisterAsync(string emailAddress, Guid siteId, string firstName, string lastName)
        {
            if (string.IsNullOrWhiteSpace(firstName))
            {
                throw new BookingWebAPIException(ApplicationErrorCodes.UserFirstNameRequired);
            }

            if (string.IsNullOrWhiteSpace(lastName))
            {
                throw new BookingWebAPIException(ApplicationErrorCodes.UserLastNameRequired);
            }

            if (!Utilities.IsValidEmail(emailAddress))
            {
                throw new BookingWebAPIException(ApplicationErrorCodes.UserEmailInvalidFormat);
            }

            if(await _userRepository.ExistsByEmailAsync(emailAddress))
            {
                throw new BookingWebAPIException(ApplicationErrorCodes.UserEmailMustBeUnique);
            }

            if(! await _siteRepository.ExistsAsync(siteId))
            {
                throw new BookingWebAPIException(ApplicationErrorCodes.SiteDoesNotExist);
            }

            var registeredUser = await _userRepository.CreateOrUpdateAsync(new BookingWebAPIUser
            {
                Email = emailAddress,
                EmailConfirmed = false,
                Token = Guid.NewGuid(),
                UserName = await ProposeUserNameAsync(firstName.Trim(), lastName.Trim()),
                FirstName = firstName.Trim(),
                LastName = lastName.Trim(),
                SiteId = siteId
            });

            _jobClient.Enqueue<IEmailService>(emailService => emailService.SendUserConfirmationEmailAsync(registeredUser.Id));

            return registeredUser;
        }

        public async Task<BookingWebAPIUser> FindUserForEmailConfirmationAsync(Guid token)
        {
            var foundUser = await _userRepository.FindByEmailVerificationTokenAsync(token);
            if(foundUser == null)
            {
                throw new BookingWebAPIException(ApplicationErrorCodes.UserDoesNotExist);
            }
            return foundUser;
        }

        public async Task<BookingWebAPIUser> ConfirmRegistrationAsync(Guid userId, Guid token, string password)
        {
            if(!await _userRepository.ExistsAsync(userId) || !await _userRepository.ExistsByEmailVerificationTokenAsync(token))
            {
                throw new BookingWebAPIException(ApplicationErrorCodes.UserDoesNotExist);
            }

            if (!await IsPasswordValidByPolicyAsync(password))
            {
                throw new BookingWebAPIException(ApplicationErrorCodes.UserPasswordNotValidByPolicy);
            }

            var confirmedUser = await _userRepository.GetAsync(userId);

            confirmedUser!.PasswordHash = BCrypt.Net.BCrypt.HashPassword(password);
            confirmedUser!.EmailConfirmed = true;
            confirmedUser!.Token = null;

            return await _userRepository.CreateOrUpdateAsync(confirmedUser);
        }

        public async Task<(BookingWebAPIUser, string)> AuthenticateAsync(string emailAddress, string password)
        {
            if (string.IsNullOrWhiteSpace(emailAddress))
            {
                throw new BookingWebAPIException(ApplicationErrorCodes.UserEmailRequired);
            }

            if (!Utilities.IsValidEmail(emailAddress)) 
            {
                throw new BookingWebAPIException(ApplicationErrorCodes.UserEmailInvalidFormat);
            }

            if (string.IsNullOrWhiteSpace(password))
            {
                throw new BookingWebAPIException(ApplicationErrorCodes.LoginPasswordRequired);
            }

            var foundUser = await _userRepository.FindByUserEmailAsync(emailAddress);

            if(foundUser == null)
            {
                throw new BookingWebAPIException(ApplicationErrorCodes.LoginInvalidUserNameOrPassword);
            }

            if(!foundUser.EmailConfirmed)
            {
                throw new BookingWebAPIException(ApplicationErrorCodes.LoginEmailNotConfirmed);
            }

            if(foundUser.LockoutEnabled)
            {
                throw new BookingWebAPIException(ApplicationErrorCodes.UserLockedOut);
            }

            bool passwordValid = BCrypt.Net.BCrypt.Verify(password, foundUser.PasswordHash);

            if(!passwordValid)
            {
                int maxLoginAttempts = await _settingService.GetValueBySettingNameAsync<int>(ApplicationConstants.LoginMaxAttempts);

                foundUser.AccessFailedCount++;
                foundUser.LockoutEnabled = foundUser.AccessFailedCount >= maxLoginAttempts;
                await _userRepository.CreateOrUpdateAsync(foundUser);

                throw new BookingWebAPIException(ApplicationErrorCodes.LoginInvalidUserNameOrPassword);
            }
            else if(foundUser.AccessFailedCount > 0)
            {
                foundUser.AccessFailedCount = 0;
                await _userRepository.CreateOrUpdateAsync(foundUser);
            }

            return (foundUser, GenerateJwtToken(foundUser.Id, foundUser.Email, foundUser.UserName));
        }

        private async Task<bool> IsPasswordValidByPolicyAsync(string password)
        {
            var policySettings = await _settingService.GetSettingsForCategoryAsync(SettingCategory.PasswordPolicy);

            var minLength = _settingService.ExtractValueFromSetting<int>(policySettings.Single(s => s.Name == ApplicationConstants.PasswordPolicyMinLength));
            var maxLength = _settingService.ExtractValueFromSetting<int>(policySettings.Single(s => s.Name == ApplicationConstants.PasswordPolicyMaxLength));
            var upperCaseLetters = _settingService.ExtractValueFromSetting<bool>(policySettings.Single(s => s.Name == ApplicationConstants.PasswordPolicyUppercaseLetter));
            var specialCharacters = _settingService.ExtractValueFromSetting<bool>(policySettings.Single(s => s.Name == ApplicationConstants.PasswordPolicySpecialCharacters));
            var digits = _settingService.ExtractValueFromSetting<bool>(policySettings.Single(s => s.Name == ApplicationConstants.PasswordPolicyDigits));

            var passwordPolicyRegex = $"^{(upperCaseLetters ? "(?=.*[A-Z])" : string.Empty)}{(digits ? "(?=.*\\d)" : string.Empty)}{(specialCharacters ? "(?=.*[^\\w\\s\\d])" : string.Empty)}.{{{minLength},{maxLength}}}$";

            return Regex.IsMatch(password, passwordPolicyRegex);
        }

        private async Task<string> ProposeUserNameAsync(string firstName, string lastName)
        {
            var usersWithSameName = await _userRepository.GetAll().CountAsync(user => user.FirstName == firstName && user.LastName == lastName);
            return $"{firstName.ToLower()}.{lastName.ToLower()}{(usersWithSameName > 0 ? usersWithSameName : string.Empty)}";
        }

        private string GenerateJwtToken(Guid userId, string userEmail, string userName)
        {
            var jwtSecret = _jwtConfiguration.Value.Secret;
            if (jwtSecret == null)
            {
                throw new BookingWebAPIException(ApplicationErrorCodes.CannotAuthenticate, "No JWT secret found for authentication.");
            }

            var jwtValidInSeconds = _jwtConfiguration.Value.ValidInSeconds;
            if (jwtValidInSeconds == null || jwtValidInSeconds <= 0)
            {
                throw new BookingWebAPIException(ApplicationErrorCodes.CannotAuthenticate, "Validity time period has not been provided appropriately.");
            }

            return Utilities.CreateJwtToken(userId, userEmail, userName, jwtValidInSeconds.Value, jwtSecret);
        }
    }
}
