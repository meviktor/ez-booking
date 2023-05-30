using BookingWebAPI.Common.Constants;
using BookingWebAPI.Common.Enums;
using BookingWebAPI.Common.ErrorCodes;
using BookingWebAPI.Common.Exceptions;
using BookingWebAPI.Common.Models;
using BookingWebAPI.DAL.Interfaces;
using BookingWebAPI.Services.Interfaces;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.IdentityModel.Tokens;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;
using System.Text.RegularExpressions;

namespace BookingWebAPI.Services
{
    internal class UserService : IUserService
    {
        private readonly IConfiguration _configuration;
        private readonly IUserRepository _userRepository;
        private readonly ISettingService _settingService;

        public UserService(IConfiguration configuration, IUserRepository userRepository, ISettingService settingService)
        {
            _configuration = configuration;
            _userRepository = userRepository;
            _settingService = settingService;
        }

        public async Task<BookingWebAPIUser> Register(string emailAddress, Guid siteId, string firstName, string lastName)
        {
            if(await _userRepository.ExistsByEmail(emailAddress))
            {
                throw new BookingWebAPIException(ApplicationErrorCodes.UserEmailMustBeUnique);
            }

            var registeredUser = await _userRepository.CreateOrUpdateAsync(new BookingWebAPIUser
            {
                Email = emailAddress,
                EmailConfirmed = false,
                Token = Guid.NewGuid(),
                UserName = await ProposeUserName(firstName.Trim(), lastName.Trim()),
                FirstName = firstName.Trim(),
                LastName = lastName.Trim(),
                SiteId = siteId
            });

            // TODO: send confirmation e-mail for the specified e-mail address!

            return registeredUser;
        }

        public async Task<BookingWebAPIUser> FindUserForEmailConfirmation(Guid token)
        {
            var foundUser = await _userRepository.FindByEmailVerificationToken(token);
            if(foundUser == null)
            {
                throw new BookingWebAPIException(ApplicationErrorCodes.EntityNotFound);
            }
            return foundUser;
        }

        public async Task<BookingWebAPIUser> ConfirmRegistration(Guid userId, Guid token, string password)
        {
            if(!await _userRepository.ExistsAsync(userId) || !await _userRepository.ExistsByEmailVerificationToken(token))
            {
                throw new BookingWebAPIException(ApplicationErrorCodes.EntityNotFound);
            }

            if (!await IsPasswordValidByPolicy(password))
            {
                throw new BookingWebAPIException(ApplicationErrorCodes.UserPasswordNotValidByPolicy);
            }

            var confirmedUser = await _userRepository.GetAsync(userId);

            confirmedUser!.PasswordHash = BCrypt.Net.BCrypt.HashPassword(password);
            confirmedUser!.EmailConfirmed = true;
            confirmedUser!.Token = null;

            return await _userRepository.CreateOrUpdateAsync(confirmedUser);
        }

        public async Task<(BookingWebAPIUser, string)> Authenticate(string emailAddress, string password)
        {
            var foundUser = await _userRepository.FindByUserEmail(emailAddress);

            if(foundUser == null)
            {
                throw new BookingWebAPIException(ApplicationErrorCodes.LoginInvalidUserNameOrPassword);
            }

            if(foundUser.LockoutEnabled)
            {
                throw new BookingWebAPIException(ApplicationErrorCodes.UserLockedOut);
            }

            bool passwordValid = BCrypt.Net.BCrypt.Verify(password, foundUser.PasswordHash);

            foundUser.AccessFailedCount = passwordValid ? 0 : foundUser.AccessFailedCount + 1;
            foundUser.LockoutEnabled = foundUser.AccessFailedCount >= ApplicationConstants.LoginMaxAttempts;
            await _userRepository.CreateOrUpdateAsync(foundUser);

            if (!passwordValid)
            {
                throw new BookingWebAPIException(ApplicationErrorCodes.LoginInvalidUserNameOrPassword);
            }

            return (foundUser, GenerateJwtToken(foundUser.Id, foundUser.Email, foundUser.UserName));
        }

        private async Task<bool> IsPasswordValidByPolicy(string password)
        {
            var policySettings = await _settingService.GetSettingsForCategory(SettingCategory.PasswordPolicy);

            var minLength = _settingService.ExtractValueFromSetting<int>(policySettings.Single(s => s.Name == ApplicationConstants.PasswordPolicyMinLength));
            var maxLength = _settingService.ExtractValueFromSetting<int>(policySettings.Single(s => s.Name == ApplicationConstants.PasswordPolicyMaxLength));
            var upperCaseLetters = _settingService.ExtractValueFromSetting<bool>(policySettings.Single(s => s.Name == ApplicationConstants.PasswordPolicyUppercaseLetter));
            var specialCharacters = _settingService.ExtractValueFromSetting<bool>(policySettings.Single(s => s.Name == ApplicationConstants.PasswordPolicySpecialCharacters));
            var digits = _settingService.ExtractValueFromSetting<bool>(policySettings.Single(s => s.Name == ApplicationConstants.PasswordPolicyDigits));

            var passwordPolicyRegex = $"^(?=.*[a-z]){(upperCaseLetters ? "(?=.*[A-Z])" : string.Empty)}{(digits ? "(?=.*\\d)" : string.Empty)}{(specialCharacters ? "(?=.*[^\\w\\d\\s])" : string.Empty)}.{{{minLength},{maxLength}}}$";

            return Regex.IsMatch(password, passwordPolicyRegex);
        }

        private async Task<string> ProposeUserName(string firstName, string lastName)
        {
            var usersWithSameName = await _userRepository.GetAll().CountAsync(user => user.FirstName == firstName && user.LastName == lastName);
            return $"{firstName.ToLower()}.{lastName.ToLower()}{(usersWithSameName > 0 ? usersWithSameName : string.Empty)}";
        }

        private string GenerateJwtToken(Guid userId, string userEmail, string userName)
        {
            var jwtSecret = _configuration["JwtConfig:Secret"];

            if(jwtSecret == null)
            {
                throw new BookingWebAPIException(ApplicationErrorCodes.CannotAuthenticate, "No JWT secret found for authentication.");
            }

            if(!int.TryParse(_configuration["JwtConfig:ValidInDays"], out int jwtValidInDays))
            {
                throw new BookingWebAPIException(ApplicationErrorCodes.CannotAuthenticate, "No validity time period found for authentication.");
            }

            var tokenHandler = new JwtSecurityTokenHandler();
            var key = Encoding.ASCII.GetBytes(jwtSecret);
            var tokenDescriptor = new SecurityTokenDescriptor
            {
                Subject = new ClaimsIdentity(new[] {
                    new Claim(ApplicationConstants.JwtClaimId, $"{userId}"),
                    new Claim(ApplicationConstants.JwtClaimEmail, $"{userEmail}"),
                    new Claim(ApplicationConstants.JwtClaimUserName, $"{userName}")
                }),
                Expires = DateTime.UtcNow.AddDays(jwtValidInDays),
                SigningCredentials = new SigningCredentials(new SymmetricSecurityKey(key), SecurityAlgorithms.HmacSha256Signature)
            };
            var token = tokenHandler.CreateToken(tokenDescriptor);
            return tokenHandler.WriteToken(token);
        }
    }
}
