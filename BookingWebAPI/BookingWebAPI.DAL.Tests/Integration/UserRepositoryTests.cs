using BookingWebAPI.Common.Constants;
using BookingWebAPI.Common.ErrorCodes;
using BookingWebAPI.Common.Exceptions;
using BookingWebAPI.Common.Models;
using BookingWebAPI.DAL.Repositories;
using BookingWebAPI.Testing.Common;
using FluentAssertions;
using Microsoft.EntityFrameworkCore;
using NUnit.Framework;

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

        [TestCase(TestDatabaseSeeder.Constants.NotExistingUserEmail, true, null)]
        [TestCase(TestDatabaseSeeder.Constants.ActiveUserEmail, false, ApplicationErrorCodes.UserEmailMustBeUnique)]
        [TestCase(TestDatabaseSeeder.Constants.DeletedUserEmail, false, ApplicationErrorCodes.UserEmailMustBeUnique)]
        [TestCase(null, false, ApplicationErrorCodes.UserEmailRequired)]
        public async Task CreateOrUpdate_Test_CreateWithEmailAddress(string emailAddress, bool operationShouldSucceed, string? expectedErrorCode)
        {
            var countOfUsersBeforeSave = await _repository.GetAll().CountAsync();
            var userToSave = new BookingWebAPIUser { Email = emailAddress, UserName = TestDatabaseSeeder.Constants.NotExistingUserUserName, PasswordHash = TestDatabaseSeeder.DummyPasswordHash };

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
            var userToSave = new BookingWebAPIUser { Email = new string('A', ApplicationConstants.EmailMaximumLength + 1), UserName = TestDatabaseSeeder.Constants.NotExistingUserUserName, PasswordHash = TestDatabaseSeeder.DummyPasswordHash };

            var assertAction = () => _repository.CreateOrUpdateAsync(userToSave);

            await assertAction.Should().ThrowExactlyAsync<DALException>().Where(e => e.ErrorCode == ApplicationErrorCodes.UserEmailTooLong);
        }

        [TestCase(TestDatabaseSeeder.Constants.NotExistingUserEmail, true, null)]
        [TestCase(null, false, ApplicationErrorCodes.UserUserNameRequired)]
        public async Task CreateOrUpdate_Test_CreateWithUserName(string userName, bool operationShouldSucceed, string? expectedErrorCode)
        {
            var countOfUsersBeforeSave = await _repository.GetAll().CountAsync();
            var userToSave = new BookingWebAPIUser { Email = TestDatabaseSeeder.Constants.NotExistingUserEmail, UserName = userName, PasswordHash = new string('A', 60) };

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
            var userToSave = new BookingWebAPIUser { Email = TestDatabaseSeeder.Constants.NotExistingUserEmail, UserName = new string('A', ApplicationConstants.UserNameMaximumLength + 1), PasswordHash = TestDatabaseSeeder.DummyPasswordHash };

            var assertAction = () => _repository.CreateOrUpdateAsync(userToSave);

            await assertAction.Should().ThrowExactlyAsync<DALException>().Where(e => e.ErrorCode == ApplicationErrorCodes.UserUserNameTooLong);
        }
    }
}
