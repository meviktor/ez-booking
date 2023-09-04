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

        [TestCase(Constants.NotExistingUserEmail, true)]
        [TestCase(Constants.ActiveUserEmail, false)]
        [TestCase(Constants.DeletedUserEmail, false)]
        public async Task CreateOrUpdate_Test_CreateWithEmailAddressUniqueConstraint(string emailAddress, bool operationShouldSucceed)
        {
            var countOfUsersBeforeSave = await _repository.GetAll().CountAsync();
            var assertAction = () => _repository.CreateOrUpdateAsync(CreateUserByField(nameof(BookingWebAPIUser.Email), emailAddress));

            if(operationShouldSucceed)
            {
                await assertAction();
                var countOfUsersAfterSave = await _repository.GetAll().CountAsync();

                countOfUsersAfterSave.Should().Be(countOfUsersBeforeSave + 1);
            }
            else await assertAction.Should().ThrowExactlyAsync<DALException>().Where(e => e.ErrorCode == ApplicationErrorCodes.UserEmailMustBeUnique);
        }

        [TestCase(nameof(BookingWebAPIUser.UserName), ApplicationConstants.UserNameMaximumLength, ApplicationErrorCodes.UserUserNameTooLong)]
        [TestCase(nameof(BookingWebAPIUser.Email), ApplicationConstants.EmailMaximumLength, ApplicationErrorCodes.UserEmailTooLong)]
        public async Task CreateOrUpdate_Test_CreateWithMaxLengthField(string fieldName, int fieldMaximumLength, string? expectedErrorCode)
        {
            var assertAction = () => _repository.CreateOrUpdateAsync(CreateUserByField(fieldName, new string('A', fieldMaximumLength + 1)));

            await assertAction.Should().ThrowExactlyAsync<DALException>().Where(e => e.ErrorCode == expectedErrorCode);
        }


        [TestCase(nameof(BookingWebAPIUser.Email), null, false, ApplicationErrorCodes.UserEmailRequired)]
        [TestCase(nameof(BookingWebAPIUser.Email), "", false, ApplicationErrorCodes.UserEmailRequired)]
        [TestCase(nameof(BookingWebAPIUser.UserName), Constants.NotRegisteredUserUserName, true, null)]
        [TestCase(nameof(BookingWebAPIUser.UserName), null, false, ApplicationErrorCodes.UserUserNameRequired)]
        [TestCase(nameof(BookingWebAPIUser.UserName), "", false, ApplicationErrorCodes.UserUserNameRequired)]
        public async Task CreateOrUpdate_Test_CreateWithRequiredField(string fieldName, object? value, bool operationShouldSucceed, string? expectedErrorCode)
        {
            var countOfUsersBeforeSave = await _repository.GetAll().CountAsync();
            var assertAction = () => _repository.CreateOrUpdateAsync(CreateUserByField(fieldName, value));

            if (operationShouldSucceed)
            {
                await assertAction();
                var countOfUsersAfterSave = await _repository.GetAll().CountAsync();

                countOfUsersAfterSave.Should().Be(countOfUsersBeforeSave + 1);
            }
            else await assertAction.Should().ThrowExactlyAsync<DALException>().Where(e => e.ErrorCode == expectedErrorCode);
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
            new BookingWebAPIUser { UserName = userName, Email = email, EmailConfirmed = emailConfirmed, PasswordHash = passwordHash ?? DummyPasswordHash, LockoutEnabled = lockoutEnabled, AccessFailedCount = accessFailedCount };
#pragma warning restore CS8601
    }
}
