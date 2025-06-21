using BookingWebAPI.Common.Enums;
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
    [TestFixture]
    internal class EmailConfirmationAttemptRepositoryTests : IntegrationTestBase
    {
        private EmailConfirmationAttemptRepository _repository;

        [SetUp]
        public override void SetUp()
        {
            base.SetUp();
            _repository = new EmailConfirmationAttemptRepository(_dbContext);
        }

        [Test]
        public async Task CreateOrUpdateAsync_Test_NoUserId()
        {
            // prepare
            var numberOfAttemptsBeforeSave = await _repository.GetAll().CountAsync();
            var action = async () => await _repository.CreateOrUpdateAsync(new EmailConfirmationAttempt { CreatedAt = DateTimeOffset.UtcNow, Status = EmailConfirmationStatus.Initiated });

            // action & assert
            await action.Should().ThrowAsync<DALException>().Where(e => e.ErrorCode == ApplicationErrorCodes.EmailConfirmationUserIdRequired);
            var numberOfAttemptsAfterSave = await _repository.GetAll().CountAsync();
            numberOfAttemptsAfterSave.Should().Be(numberOfAttemptsBeforeSave);
        }

        [TestCase(EmailConfirmationStatus.Initiated, EmailConfirmationStatus.Initiated, TestDatabaseSeeder.Constants.ActiveUserId, TestDatabaseSeeder.Constants.ActiveUserId)]
        [TestCase(EmailConfirmationStatus.Initiated, EmailConfirmationStatus.Succeeded, TestDatabaseSeeder.Constants.ActiveUserId, TestDatabaseSeeder.Constants.ActiveUserId)]
        [TestCase(EmailConfirmationStatus.Initiated, EmailConfirmationStatus.Initiated, TestDatabaseSeeder.Constants.ActiveUserId, TestDatabaseSeeder.Constants.DeletedUserId)]
        [TestCase(EmailConfirmationStatus.Initiated, EmailConfirmationStatus.InProgress, TestDatabaseSeeder.Constants.ActiveUserId, TestDatabaseSeeder.Constants.DeletedUserId)]
        public async Task GetByStatusAsync_Test(EmailConfirmationStatus statusForSave, EmailConfirmationStatus statusForSearch, string userIdForSave, string userIdForSearch)
        {
            // prepare
            var numberOfAttemptsBeforeSave = await _repository.GetAll().CountAsync();
            var savedAttempt = await _repository.CreateOrUpdateAsync(new EmailConfirmationAttempt { CreatedAt = DateTimeOffset.UtcNow, Status = statusForSave, UserId = Guid.Parse(userIdForSearch) });

            var numberOfAttemptsAfterSave = await _repository.GetAll().CountAsync();
            numberOfAttemptsAfterSave.Should().Be(numberOfAttemptsBeforeSave + 1);

            // action
            var foundAttempts = await _repository.GetByStatusAsync(Guid.Parse(userIdForSearch), statusForSearch);

            // assert
            foundAttempts.All(a => a.UserId == Guid.Parse(userIdForSearch)).Should().BeTrue();
            foundAttempts.All(a => a.Status == statusForSearch).Should().BeTrue();
            if (statusForSave == statusForSearch && userIdForSave == userIdForSearch)
            {
                foundAttempts.SingleOrDefault(a => a.Id == savedAttempt.Id).Should().NotBeNull();
            }
        }
    }
}
