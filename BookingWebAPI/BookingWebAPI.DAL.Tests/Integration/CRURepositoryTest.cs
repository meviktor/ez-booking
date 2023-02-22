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
    internal class CRURepositoryTest : IntegrationTestBase
    {
        private CRURepository<Site> _repository;
        private Site _siteForAdding;

        [SetUp]
        public override void SetUp()
        {
            base.SetUp();
            _repository = new CRURepository<Site>(_dbContext);
            _siteForAdding = new Site
            {
                Id = default,
                Name = "Test site",
                Country = "Test country",
                ZipCode = "TEST0001",
                City = "Test City",
                County = "Test County",
                Street = "Test St.",
                HouseOrFlatNumber = "1a"
            };
        }

        [Test]
        public async Task CreateOrUpdateAsync_Test_UpdateExistingEntity()
        {
            var newSiteDescription = $"Site description modified by {nameof(CRURepositoryTest)}.";
            var activeSite = await _repository.GetAsync(new Guid(TestDatabaseSeeder.SiteConstants.ActiveSiteId));

            activeSite.Description = newSiteDescription;
            var updatedActiveSite = await _repository.CreateOrUpdateAsync(activeSite);

            updatedActiveSite.Description.Should().Be(newSiteDescription);
        }

        [Test]
        public async Task CreateOrUpdateAsync_Test_CreateEntity()
        {
            var numberOfAlredyExistingSites = await _repository.GetAll().CountAsync();

            var newSite = await _repository.CreateOrUpdateAsync(_siteForAdding);
            var numberOfSitesAfterCreate = await _repository.GetAll().CountAsync();

            newSite.Name.Should().Be(_siteForAdding.Name);
            newSite.Country.Should().Be(_siteForAdding.Country);
            newSite.ZipCode.Should().Be(_siteForAdding.ZipCode);
            newSite.City.Should().Be(_siteForAdding.City);
            newSite.Street.Should().Be(_siteForAdding.Street);
            newSite.HouseOrFlatNumber.Should().Be(_siteForAdding.HouseOrFlatNumber);
            numberOfSitesAfterCreate.Should().Be(numberOfAlredyExistingSites + 1);
        }

        [Test]
        public async Task CreateOrUpdateAsync_Test_CreateEntityWithNotExistingId()
        {
            _siteForAdding.Id = Guid.NewGuid();

            var action = () => _repository.CreateOrUpdateAsync(_siteForAdding);

            await action.Should().ThrowExactlyAsync<DALException>().Where(e => e.ErrorCode == ApplicationErrorCodes.EntityNotFound);
        }

        [Test]
        public async Task CreateOrUpdateAsync_Test_CreateEntityWithDeletedId()
        {
            _siteForAdding.Id = new Guid(TestDatabaseSeeder.SiteConstants.DeletedSiteId);

            var action = () => _repository.CreateOrUpdateAsync(_siteForAdding);

            await action.Should().ThrowExactlyAsync<DALException>().Where(e => e.ErrorCode == ApplicationErrorCodes.EntityNotFound);
        }
    }
}
