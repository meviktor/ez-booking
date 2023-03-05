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
    internal class CRUDRepositoryTests : IntegrationTestBase
    {
        private CRUDRepository<Site> _repository;

        [SetUp]
        public override void SetUp()
        {
            base.SetUp();
            _repository = new CRUDRepository<Site>(_dbContext);
        }

        [TestCase(TestDatabaseSeeder.Constants.EmptyId)]
        [TestCase(TestDatabaseSeeder.Constants.DeletedSiteId)]
        public async Task DeleteAsync_Test_CallWithNotExistingId(string idAsString)
        {
            var action = () => _repository.DeleteAsync(new Guid(idAsString));

            await action.Should().ThrowExactlyAsync<DALException>().Where(e => e.ErrorCode == ApplicationErrorCodes.EntityNotFound);
        }

        [Test]
        public async Task DeleteAsync_Test_CallWithExistingId()
        {
            var entriesBeforeDelete = await _repository.GetAll().CountAsync();

            var activeSiteId = new Guid(TestDatabaseSeeder.Constants.ActiveSiteId);
            await _repository.DeleteAsync(activeSiteId);

            var entriesAfterDelete = await _repository.GetAll().CountAsync();
            var assertAction = () => _repository.GetAsync(activeSiteId);

            entriesAfterDelete.Should().Be(entriesBeforeDelete - 1);
            await assertAction.Should().ThrowExactlyAsync<DALException>().Where(e => e.ErrorCode == ApplicationErrorCodes.EntityNotFound);
        }
    }
}
