using BookingWebAPI.Common.Constants;
using BookingWebAPI.Common.Enums;
using BookingWebAPI.Common.ErrorCodes;
using BookingWebAPI.Common.Exceptions;
using BookingWebAPI.Common.Models;
using BookingWebAPI.Common.Models.Config;
using BookingWebAPI.Common.Utils;
using BookingWebAPI.DAL;
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
        private readonly IEmailConfirmationAttemptService _emailConfirmationService;
        // TODO: instead of using the DbContext directly a dedicated interface would be nice for only the specific needed operations, e.g. transaction handling
        private readonly BookingWebAPIDbContext _dbContext;

        public UserService(IOptions<JwtConfiguration> jwtConfiguration, IUserRepository userRepository, ISettingService settingService, IBackgroundJobClient jobClient, ISiteRepository siteRepository, IEmailConfirmationAttemptService emailConfirmationService, BookingWebAPIDbContext dbContext)
        {
            _jwtConfiguration = jwtConfiguration;
            _userRepository = userRepository;
            _settingService = settingService;
            _jobClient = jobClient;
            _siteRepository = siteRepository;
            _emailConfirmationService = emailConfirmationService;
            _dbContext = dbContext;
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

            BookingWebAPIUser registeredUser;
            int minLength = await _settingService.GetValueBySettingNameAsync<int>(ApplicationConstants.PasswordPolicyMinLength);
            var tempKey = Utilities.RandomString(minLength, true, true, true);
            using (var registerUserTransaction = _dbContext.Database.BeginTransaction()) 
            {
                registeredUser = await _userRepository.CreateOrUpdateAsync(new BookingWebAPIUser
                {
                    Email = emailAddress,
                    EmailConfirmed = false,
                    UserName = await ProposeUserNameAsync(firstName.Trim(), lastName.Trim()),
                    FirstName = firstName.Trim(),
                    LastName = lastName.Trim(),
                    PasswordHash = BCrypt.Net.BCrypt.HashPassword(tempKey),
                    SiteId = siteId
                });
                await _emailConfirmationService.CreateOrUpdateAsync(new EmailConfirmationAttempt
                {
                    UserId = registeredUser.Id,
                    CreatedAt = DateTimeOffset.UtcNow,
                    Status = EmailConfirmationStatus.Initiated
                });
                registerUserTransaction.Commit();
            }

            _jobClient.Enqueue<IEmailService>(emailService => emailService.SendUserConfirmationEmailAsync(registeredUser.Id, tempKey));

            return registeredUser;
        }

        public async Task<Guid> ConfirmEmailRegistrationAsync(Guid attemptId)
        {
            var (attempt, userToConfirm) = await GetValidatedEmailConfirmationAttemptAndUserAsync(attemptId);

            userToConfirm.EmailConfirmed = true;
            attempt.Status = EmailConfirmationStatus.Succeeded;

            BookingWebAPIUser user;
            using (var transaction = _dbContext.Database.BeginTransaction())
            {
                user = await _userRepository.CreateOrUpdateAsync(userToConfirm);
                await _emailConfirmationService.CreateOrUpdateAsync(attempt);
            }

            return user.Id;
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

        private async Task<(EmailConfirmationAttempt, BookingWebAPIUser)> GetValidatedEmailConfirmationAttemptAndUserAsync(Guid confirmationAttemptId)
        {
            var confirmationAttempt = await _emailConfirmationService.GetAsync(confirmationAttemptId);
            // Only those attempts are accepted whose email has been already sent out to the user (so NOT 'Initiated'), are in-progress (so, NOT yet 'Succeeded' or 'Failed') and hasn't been expired.
            if (confirmationAttempt == null || confirmationAttempt.Status != EmailConfirmationStatus.InProgress)
            {
                throw new BookingWebAPIException(ApplicationErrorCodes.EmailConfirmationInvalidAttempt);
            }
            if (confirmationAttempt.CreatedAt.AddHours(ApplicationConstants.ActivationLinkExpirationHours) < DateTimeOffset.UtcNow)
            {
                confirmationAttempt.Status = EmailConfirmationStatus.Failed;
                confirmationAttempt.FailReason = EmailConfirmationFailReason.Expired;
                await _emailConfirmationService.CreateOrUpdateAsync(confirmationAttempt);

                throw new BookingWebAPIException(ApplicationErrorCodes.EmailConfirmationLinkExpired);
            }

            var foundUser = await _userRepository.GetAsync(confirmationAttempt.UserId);
            if (foundUser == null)
            {
                throw new BookingWebAPIException(ApplicationErrorCodes.UserDoesNotExist);
            }
            if (foundUser.EmailConfirmed)
            {
                throw new BookingWebAPIException(ApplicationErrorCodes.UserEmailAlreadyConfirmed);
            }

            return (confirmationAttempt, foundUser);
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
