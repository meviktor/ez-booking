using BookingWebAPI.Common.Constants;
using BookingWebAPI.Common.ErrorCodes;
using BookingWebAPI.Common.Exceptions;
using BookingWebAPI.Common.Models;
using BookingWebAPI.DAL.Repositories;
using BookingWebAPI.Testing.Common;
using FluentAssertions;
using Microsoft.EntityFrameworkCore;
using NUnit.Framework;
using System.Diagnostics.Metrics;
using System.IO;
using System.Reflection.Emit;
using static BookingWebAPI.Testing.Common.TestDatabaseSeeder;

namespace BookingWebAPI.DAL.Tests.Integration
{
    internal class SiteRepositoryTests : IntegrationTestBase
    {
        private SiteRepository _repository;

        [SetUp]
        public override void SetUp()
        {
            base.SetUp();
            _repository = new SiteRepository(_dbContext);
        }

        [TestCase(Constants.NotExistingSiteName, true, null)]
        [TestCase(Constants.ActiveSiteName, false, ApplicationErrorCodes.SiteNameMustBeUnique)]
        [TestCase(Constants.DeletedSiteName, false, ApplicationErrorCodes.SiteNameMustBeUnique)]
        [TestCase(null, false, ApplicationErrorCodes.SiteNameRequired)]
        public async Task CreateOrUpdate_Test_CreateWithName(string name, bool operationShouldSucceed, string? expectedErrorCode)
        {
            var countOfSitesBeforeSave = await _repository.GetAll().CountAsync();

            var assertAction = () => _repository.CreateOrUpdateAsync(CreateSite(name: name));

            if (operationShouldSucceed)
            {
                await assertAction();
                var countOfSitesAfterSave = await _repository.GetAll().CountAsync();

                countOfSitesAfterSave.Should().Be(countOfSitesBeforeSave + 1);
            }
            else await assertAction.Should().ThrowExactlyAsync<DALException>().Where(e => e.ErrorCode == expectedErrorCode);
        }

        [Test]
        public async Task CreateOrUpdate_Test_CreateWithLongEmailName()
        {
            var assertAction = () => _repository.CreateOrUpdateAsync(CreateSite(name: new string('A', ApplicationConstants.SiteNameMaximiumLength + 1)));

            await assertAction.Should().ThrowExactlyAsync<DALException>().Where(e => e.ErrorCode == ApplicationErrorCodes.SiteNameTooLong);
        }

        [TestCase("")]
        [TestCase(null)]
        public async Task CreateOrUpdate_Test_CreateWithDescription(string? description)
        {
            var countOfSitesBeforeSave = await _repository.GetAll().CountAsync();

            var assertAction = () => _repository.CreateOrUpdateAsync(CreateSite(description: description));

            await assertAction();
            var countOfSitesAfterSave = await _repository.GetAll().CountAsync();

            countOfSitesAfterSave.Should().Be(countOfSitesBeforeSave + 1);
        }

        [Test]
        public async Task CreateOrUpdate_Test_CreateWithLongDescription()
        {
            var assertAction = () => _repository.CreateOrUpdateAsync(CreateSite(description: new string('A', ApplicationConstants.SiteDescriptionMaximiumLength + 1)));

            await assertAction.Should().ThrowExactlyAsync<DALException>().Where(e => e.ErrorCode == ApplicationErrorCodes.SiteDescriptionTooLong);
        }

        private Site CreateSite(string name = Constants.NotExistingSiteName, string country = "", string? state = null, string county = "", string city = "", string street = "", string houseOrFlatNumber = "", string zipCode = "", string? description = null, bool isDeleted = false) =>
            new Site { Name = name, Country = country, State = state, County = county, City = city, Street = street, HouseOrFlatNumber = houseOrFlatNumber, ZipCode = zipCode, Description = description, IsDeleted = isDeleted };
    }
}
