using BookingWebAPI.Common.Constants;
using BookingWebAPI.Common.ErrorCodes;
using BookingWebAPI.Common.Exceptions;
using BookingWebAPI.Common.Models;
using BookingWebAPI.Common.Models.Config;
using BookingWebAPI.DAL.Interfaces;
using BookingWebAPI.DAL.Repositories;
using BookingWebAPI.Services.Interfaces;
using BookingWebAPI.Testing.Common;
using FluentAssertions;
using Hangfire;
using Microsoft.Extensions.Options;
using Moq;
using NUnit.Framework;

namespace BookingWebAPI.Services.Tests.Integration
{
    internal class UserServiceTests : IntegrationTestBase
    {
        private IUserService _userService;
        private IUserRepository _userRepository;
        private ISettingService _settingService;
        private IEmailConfirmationAttemptService _emailConfirmationAttemptService;

        [SetUp]
        public override void SetUp()
        {
            base.SetUp();
            var jwtOptions = Options.Create(new JwtConfiguration { Secret = "thisIsASecretForTheUserServiceTestsClass", ValidInSeconds = 3600 });
            // We do not intend testing Hangfire dependent functionality (yet)
            var hangfireMock = new Mock<IBackgroundJobClient>();
            _userRepository = new UserRepository(_dbContext);
            _settingService = new SettingService(new SettingRepository(_dbContext));
            _emailConfirmationAttemptService = new EmailConfirmationAttemptService(new EmailConfirmationAttemptRepository(_dbContext));
            _userService = new UserService(jwtOptions, _userRepository, _settingService, hangfireMock.Object, new SiteRepository(_dbContext), _emailConfirmationAttemptService, _dbContext);
        }

        [Test]
        public async Task Authenticate_Test_AccessFailedCount_Increment()
        {
            // prepare
            var lockoutTreshold = await _settingService.GetValueBySettingNameAsync<int>(ApplicationConstants.LoginMaxAttempts);
            var emailToAuthenticate = "testing@mymail.com";
            var passwordToAuthenticate = "authPwd!";
            var testUser = await _userRepository.CreateOrUpdateAsync(new BookingWebAPIUser
            {
                Email = emailToAuthenticate,
                UserName = "Test",
                FirstName = "Test",
                LastName = "Test",
                EmailConfirmed = true,
                LockoutEnabled = false,
                AccessFailedCount = lockoutTreshold - 1,
                PasswordHash = BCrypt.Net.BCrypt.HashPassword($"{passwordToAuthenticate}ruinPassword"),
                SiteId = Guid.Parse(TestDatabaseSeeder.Constants.ActiveSiteId)
            });

            // action & assert
            var action = () => _userService.AuthenticateAsync(emailToAuthenticate, passwordToAuthenticate);
            await action.Should().ThrowExactlyAsync<BookingWebAPIException>().Where(e => e.ErrorCode == ApplicationErrorCodes.LoginInvalidUserNameOrPassword);
            // number of max login attempts reached... user has been locked out
            await action.Should().ThrowExactlyAsync<BookingWebAPIException>().Where(e => e.ErrorCode == ApplicationErrorCodes.UserLockedOut);
        }

        [Test]
        public async Task Authenticate_Test_AccessFailedCount_SetToZero()
        {
            // prepare
            var lockoutTreshold = await _settingService.GetValueBySettingNameAsync<int>(ApplicationConstants.LoginMaxAttempts);
            var emailToAuthenticate = "testing@mymail.com";
            var passwordToAuthenticate = "authPwd!";
            await _userRepository.CreateOrUpdateAsync(new BookingWebAPIUser
            {
                Email = emailToAuthenticate,
                UserName = "Test",
                FirstName = "Test",
                LastName = "Test",
                EmailConfirmed = true,
                LockoutEnabled = false,
                AccessFailedCount = lockoutTreshold - 1,
                PasswordHash = BCrypt.Net.BCrypt.HashPassword(passwordToAuthenticate),
                SiteId = Guid.Parse(TestDatabaseSeeder.Constants.ActiveSiteId)
            });

            // action & assert
            var (userAfterLogin, _) = await _userService.AuthenticateAsync(emailToAuthenticate, passwordToAuthenticate);
            userAfterLogin.AccessFailedCount.Should().Be(0);
        }
    }
}
