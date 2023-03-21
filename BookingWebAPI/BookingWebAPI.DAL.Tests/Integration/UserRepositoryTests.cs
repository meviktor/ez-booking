using BookingWebAPI.Common.Constants;
using BookingWebAPI.Common.ErrorCodes;
using BookingWebAPI.Common.Exceptions;
using BookingWebAPI.Common.Models;
using BookingWebAPI.DAL.Repositories;
using BookingWebAPI.Testing.Common;
using FluentAssertions;
using Microsoft.EntityFrameworkCore;
using NUnit.Framework;
using static BookingWebAPI.Testing.Common.TestDatabaseSeeder;

namespace BookingWebAPI.DAL.Tests.Integration
{
    internal class UserRepositoryTests : IntegrationTestBase
    {
        private UserRepository _repository;

        [SetUp]
        public override void SetUp()
        {
            base.SetUp();
            _repository = new UserRepository(_dbContext);
        }

        [TestCase(Constants.NotExistingUserEmail, true, null)]
        [TestCase(Constants.ActiveUserEmail, false, ApplicationErrorCodes.UserEmailMustBeUnique)]
        [TestCase(Constants.DeletedUserEmail, false, ApplicationErrorCodes.UserEmailMustBeUnique)]
        [TestCase(null, false, ApplicationErrorCodes.UserEmailRequired)]
        [TestCase("", false, ApplicationErrorCodes.UserEmailRequired)]
        public async Task CreateOrUpdate_Test_CreateWithEmailAddress(string emailAddress, bool operationShouldSucceed, string? expectedErrorCode)
        {
            var countOfUsersBeforeSave = await _repository.GetAll().CountAsync();
            var userToSave = new BookingWebAPIUser { Email = emailAddress, UserName = Constants.NotExistingUserUserName, PasswordHash = DummyPasswordHash };

            var assertAction = () => _repository.CreateOrUpdateAsync(userToSave);

            if(operationShouldSucceed)
            {
                await assertAction();
                var countOfUsersAfterSave = await _repository.GetAll().CountAsync();

                countOfUsersAfterSave.Should().Be(countOfUsersBeforeSave + 1);
            }
            else await assertAction.Should().ThrowExactlyAsync<DALException>().Where(e => e.ErrorCode == expectedErrorCode);
        }

        [Test]
        public async Task CreateOrUpdate_Test_CreateWithLongEmailAddress()
        {
            var userToSave = new BookingWebAPIUser { Email = new string('A', ApplicationConstants.EmailMaximumLength + 1), UserName = Constants.NotExistingUserUserName, PasswordHash = DummyPasswordHash };

            var assertAction = () => _repository.CreateOrUpdateAsync(userToSave);

            await assertAction.Should().ThrowExactlyAsync<DALException>().Where(e => e.ErrorCode == ApplicationErrorCodes.UserEmailTooLong);
        }

        [TestCase(Constants.NotExistingUserEmail, true, null)]
        [TestCase(null, false, ApplicationErrorCodes.UserUserNameRequired)]
        [TestCase("", false, ApplicationErrorCodes.UserUserNameRequired)]
        public async Task CreateOrUpdate_Test_CreateWithUserName(string userName, bool operationShouldSucceed, string? expectedErrorCode)
        {
            var countOfUsersBeforeSave = await _repository.GetAll().CountAsync();
            var userToSave = new BookingWebAPIUser { Email = Constants.NotExistingUserEmail, UserName = userName, PasswordHash = new string('A', 60) };

            var assertAction = () => _repository.CreateOrUpdateAsync(userToSave);

            if (operationShouldSucceed)
            {
                await assertAction();
                var countOfUsersAfterSave = await _repository.GetAll().CountAsync();

                countOfUsersAfterSave.Should().Be(countOfUsersBeforeSave + 1);
            }
            else await assertAction.Should().ThrowExactlyAsync<DALException>().Where(e => e.ErrorCode == expectedErrorCode);
        }

        [Test]
        public async Task CreateOrUpdate_Test_CreateWithLongUserName()
        {
            var userToSave = new BookingWebAPIUser { Email = Constants.NotExistingUserEmail, UserName = new string('A', ApplicationConstants.UserNameMaximumLength + 1), PasswordHash = DummyPasswordHash };

            var assertAction = () => _repository.CreateOrUpdateAsync(userToSave);

            await assertAction.Should().ThrowExactlyAsync<DALException>().Where(e => e.ErrorCode == ApplicationErrorCodes.UserUserNameTooLong);
        }

        private BookingWebAPIUser CreateUserByField(string fieldName, object? value) => fieldName switch
        {
            nameof(BookingWebAPIUser.UserName) => CreateUser(userName: (string?)value),
            nameof(BookingWebAPIUser.Email) => CreateUser(email: (string?)value),
            nameof(BookingWebAPIUser.EmailConfirmed) => CreateUser(emailConfirmed: (bool?)value ?? false),
            nameof(BookingWebAPIUser.PasswordHash) => CreateUser(passwordHash: (string?)value),
            nameof(BookingWebAPIUser.LockoutEnabled) => CreateUser(lockoutEnabled: (bool?)value ?? false),
            nameof(BookingWebAPIUser.AccessFailedCount) => CreateUser(accessFailedCount: (int?)value ?? 0),
            _ => throw new ArgumentException($"Field '{fieldName}' is not known on type '{nameof(BookingWebAPIUser)}'.")
        };

#pragma warning disable CS8601
        /// <summary>
        /// Creates a user with the configured properties. Pragma warning is used because cases with null property values are also tested regardless of the exact property type (nullable or not). 
        /// </summary>
        /// <returns>A <see cref="BookingWebAPIUser"/> with the configured properties.</returns>
        private BookingWebAPIUser CreateUser(string? userName = Constants.NotExistingSiteName, string? email = Constants.NotExistingUserEmail, bool emailConfirmed = false, string? passwordHash = null, bool lockoutEnabled = true, int accessFailedCount = 0) =>
            new BookingWebAPIUser { UserName = userName, Email = email, EmailConfirmed = emailConfirmed, PasswordHash = passwordHash, LockoutEnabled = lockoutEnabled, AccessFailedCount = accessFailedCount };
#pragma warning restore CS8601
    }
}
