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

        [TestCase(Constants.NotRegisteredUserEmail, true)]
        [TestCase(Constants.ActiveUserEmail, false)]
        [TestCase(Constants.DeletedUserEmail, false)]
        public async Task CreateOrUpdateAsync_Test_CreateWithEmailAddressUniqueConstraint(string emailAddress, bool operationShouldSucceed)
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
        public async Task CreateOrUpdateAsync_Test_CreateWithMaxLengthField(string fieldName, int fieldMaximumLength, string? expectedErrorCode)
        {
            var assertAction = () => _repository.CreateOrUpdateAsync(CreateUserByField(fieldName, new string('A', fieldMaximumLength + 1)));

            await assertAction.Should().ThrowExactlyAsync<DALException>().Where(e => e.ErrorCode == expectedErrorCode);
        }

        [TestCase(nameof(BookingWebAPIUser.Email), null, false, ApplicationErrorCodes.UserEmailRequired)]
        [TestCase(nameof(BookingWebAPIUser.Email), "", false, ApplicationErrorCodes.UserEmailRequired)]
        [TestCase(nameof(BookingWebAPIUser.UserName), Constants.NotRegisteredUserUserName, true, null)]
        [TestCase(nameof(BookingWebAPIUser.UserName), null, false, ApplicationErrorCodes.UserUserNameRequired)]
        [TestCase(nameof(BookingWebAPIUser.UserName), "", false, ApplicationErrorCodes.UserUserNameRequired)]
        [TestCase(nameof(BookingWebAPIUser.FirstName), "", false, ApplicationErrorCodes.UserFirstNameRequired)]
        [TestCase(nameof(BookingWebAPIUser.FirstName), null, false, ApplicationErrorCodes.UserFirstNameRequired)]
        [TestCase(nameof(BookingWebAPIUser.LastName), "", false, ApplicationErrorCodes.UserLastNameRequired)]
        [TestCase(nameof(BookingWebAPIUser.LastName), null, false, ApplicationErrorCodes.UserLastNameRequired)]
        [TestCase(nameof(BookingWebAPIUser.SiteId), "", false, ApplicationErrorCodes.UserSiteIdRequired)]
        [TestCase(nameof(BookingWebAPIUser.SiteId), null, false, ApplicationErrorCodes.UserSiteIdRequired)]
        public async Task CreateOrUpdateAsync_Test_CreateWithRequiredField(string fieldName, object? value, bool operationShouldSucceed, string? expectedErrorCode)
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

        [TestCase(Constants.ActiveUserEmail, true)]
        [TestCase(Constants.DeletedUserEmail, false)]
        [TestCase("", false)]
        [TestCase(null, false)]
        public async Task FindByUserEmailAsync_Test(string emailAddress, bool successExpected)
        {
            // no prepare - looking for a user who has been added by TestDatabaseSeeder...
            // action
            var userFound = await _repository.FindByUserEmailAsync(emailAddress);

            // assert
            if (successExpected)
            {
                userFound.Should().NotBeNull();
                userFound!.Email.Should().Be(emailAddress);
            }
            else userFound.Should().BeNull();
        }

        [Ignore("Implementation has to be fixed!")]
        [TestCase(true, true, false)]
        [TestCase(true, false, true)]
        [TestCase(false, true, false)]
        [TestCase(false, false, false)]
        public async Task FindByEmailVerificationTokenAsync_Test(bool userActive, bool emailConfirmed, bool successExpected)
        {
            //// prepare
            //var token = Guid.NewGuid();
            //await _repository.CreateOrUpdateAsync(CreateUser(email: Constants.NotRegisteredUserEmail, emailConfirmed: emailConfirmed, token: token, deleted: !userActive));

            //// action
            //var userFound = await _repository.FindByEmailVerificationTokenAsync(token);

            //// assert
            //if (successExpected)
            //{
            //    userFound.Should().NotBeNull();
            //    userFound!.Token.Should().Be(token);
            //}
            //else userFound.Should().BeNull();
        }

        [Ignore("Implementation has to be fixed!")]
        [TestCase(true, true, false)]
        [TestCase(true, false, true)]
        [TestCase(false, true, false)]
        [TestCase(false, false, false)]
        public async Task ExistsByEmailVerificationTokenAsync_Test(bool userActive, bool emailConfirmed, bool successExpected)
        {
            //// prepare
            //var token = Guid.NewGuid();
            //await _repository.CreateOrUpdateAsync(CreateUser(email: Constants.NotRegisteredUserEmail, emailConfirmed: emailConfirmed, token: token, deleted: !userActive));

            //// action
            //var userFound = await _repository.ExistsByEmailVerificationTokenAsync(token);

            //// assert
            //if (successExpected)
            //{
            //    userFound.Should().BeTrue();
            //}
            //else userFound.Should().BeFalse();
        }

        [TestCase(Constants.ActiveUserEmail, true)]
        [TestCase(Constants.NotRegisteredUserEmail, false)]
        [TestCase("", false)]
        [TestCase(null, false)]
        public async Task ExistsByEmailAsync_Test(string emailAddress, bool successExpected)
        {
            // action
            var userFound = await _repository.ExistsByEmailAsync(emailAddress);

            // assert
            if (successExpected)
            {
                userFound.Should().BeTrue();
            }
            else userFound.Should().BeFalse();
        }

        [TestCase(true, true)]
        [TestCase(false, false)]
        public async Task ExistsByUserNameAsync_Test(bool activeUser, bool successExpected)
        {
            // prepare
            var targetEmail = activeUser ? Constants.ActiveUserEmail : Constants.DeletedUserEmail;
            var targetUser = await _dbContext.Users.Where(user => user.Email == targetEmail).SingleOrDefaultAsync();

            if (targetUser == null || targetUser.Email != targetEmail)
            {
                Assert.Fail($"{(activeUser ? "Active" : "Deleted")} target user could not be set.");
            }

            // action
            var userFound = await _repository.ExistsByUserNameAsync(targetUser!.UserName);

            // assert
            if (successExpected)
            {
                userFound.Should().BeTrue();
            }
            else userFound.Should().BeFalse();
        }

        private BookingWebAPIUser CreateUserByField(string fieldName, object? value) => fieldName switch
        {
            nameof(BookingWebAPIUser.UserName) => CreateUser(userName: (string?)value),
            nameof(BookingWebAPIUser.Email) => CreateUser(email: (string?)value),
            nameof(BookingWebAPIUser.EmailConfirmed) => CreateUser(emailConfirmed: (bool?)value ?? false),
            nameof(BookingWebAPIUser.PasswordHash) => CreateUser(passwordHash: (string?)value),
            nameof(BookingWebAPIUser.LockoutEnabled) => CreateUser(lockoutEnabled: (bool?)value ?? false),
            nameof(BookingWebAPIUser.AccessFailedCount) => CreateUser(accessFailedCount: (int?)value ?? 0),
            nameof(BookingWebAPIUser.FirstName) => CreateUser(firstName: (string?)value),
            nameof(BookingWebAPIUser.LastName) => CreateUser(lastName: (string?)value),
            nameof(BookingWebAPIUser.SiteId) => CreateUser(siteId: (string?)value),
            _ => throw new ArgumentException($"Field '{fieldName}' is not known on type '{nameof(BookingWebAPIUser)}'.")
        };

#pragma warning disable CS8601
        /// <summary>
        /// Creates a user with the configured properties. Pragma warning is used because cases with null property values are also tested regardless of the exact property type (nullable or not). 
        /// </summary>
        /// <returns>A <see cref="BookingWebAPIUser"/> with the configured properties.</returns>
        private static BookingWebAPIUser CreateUser(string? userName = Constants.NotExistingSiteName, string? email = Constants.NotRegisteredUserEmail, bool emailConfirmed = false, string? passwordHash = null, bool lockoutEnabled = true, int accessFailedCount = 0, string? firstName = "Jane", string? lastName = "Doe", string? siteId = Constants.ActiveSiteId, Guid? token = null, bool deleted = false) =>
            new BookingWebAPIUser { UserName = userName, Email = email, EmailConfirmed = emailConfirmed, PasswordHash = passwordHash ?? DummyPasswordHash, LockoutEnabled = lockoutEnabled, AccessFailedCount = accessFailedCount, FirstName = firstName, LastName = lastName, SiteId = !string.IsNullOrWhiteSpace(siteId) ? Guid.Parse(siteId) : Guid.Empty, IsDeleted = deleted };
#pragma warning restore CS8601
    }
}
