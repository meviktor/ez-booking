using BookingWebAPI.Common.Constants;
using BookingWebAPI.Common.Enums;
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
using Microsoft.EntityFrameworkCore;
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
        private IEmailConfirmationAttemptRepository _emailConfirmationAttemptRepository;
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
            _emailConfirmationAttemptRepository = new EmailConfirmationAttemptRepository(_dbContext);
            _emailConfirmationAttemptService = new EmailConfirmationAttemptService(_emailConfirmationAttemptRepository);
            _userService = new UserService(jwtOptions, _userRepository, _settingService, hangfireMock.Object, new SiteRepository(_dbContext), _emailConfirmationAttemptService, _transactionManager);
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

        [Test]
        public async Task RegisterAsync_Test_EntitiesCreated()
        {
            // prepare
            var numberOfUsersBeforeSave = await _userRepository.GetAll().CountAsync();
            var numberOfEmailConfirmationAttemptsBeforeSave = await _emailConfirmationAttemptRepository.GetAll().CountAsync();

            // action
            var registeredUser = await _userService.RegisterAsync(TestDatabaseSeeder.Constants.NotRegisteredUserEmail, Guid.Parse(TestDatabaseSeeder.Constants.ActiveSiteId), "New", "User");

            // assert
            var numberOfUsersAfterSave = await _userRepository.GetAll().CountAsync();
            var numberOfEmailConfirmationAttemptsAfterSave = await _emailConfirmationAttemptRepository.GetAll().CountAsync();

            registeredUser.Email.Should().Be(TestDatabaseSeeder.Constants.NotRegisteredUserEmail);
            registeredUser.EmailConfirmed.Should().BeFalse();
            numberOfUsersAfterSave.Should().Be(numberOfUsersBeforeSave + 1);

            var registeredUsersAttempt = _emailConfirmationAttemptService.GetByStatusAsync(registeredUser.Id, EmailConfirmationStatus.Initiated);
            registeredUsersAttempt.Should().NotBeNull();
            numberOfEmailConfirmationAttemptsAfterSave.Should().Be(numberOfEmailConfirmationAttemptsBeforeSave + 1);
        }
    }
}
