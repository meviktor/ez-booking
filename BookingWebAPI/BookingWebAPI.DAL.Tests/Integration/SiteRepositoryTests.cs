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
    [TestFixture]
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
        public async Task CreateOrUpdateAsync_Test_CreateWithSiteNameUniqueConstraint(string name, bool operationShouldSucceed, string? expectedErrorCode)
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

        [TestCase(null, null, false)]
        [TestCase("", null, false)]
        [TestCase(null, "", false)]
        [TestCase("", "", false)]
        [TestCase("Idaho", "", true)]
        [TestCase("", "Montgomery", true)]
        [TestCase("Idaho", null, true)]
        [TestCase(null, "Montgomery", true)]
        [TestCase("Mississippi", "Montgomery", true)]
        public async Task CreateOrUpdateAsync_Test_CreateWithStateCountyCheckContraint(string state, string county, bool operationShouldSucceed)
        {
            var countOfSitesBeforeSave = await _repository.GetAll().CountAsync();

            var assertAction = () => _repository.CreateOrUpdateAsync(CreateSite(state: state, county: county));

            if (operationShouldSucceed)
            {
                await assertAction();
                var countOfSitesAfterSave = await _repository.GetAll().CountAsync();

                countOfSitesAfterSave.Should().Be(countOfSitesBeforeSave + 1);
            }
            else await assertAction.Should().ThrowExactlyAsync<DALException>().Where(e => e.ErrorCode == ApplicationErrorCodes.SiteStateOrCountryNeeded);
        }

        [TestCase(nameof(Site.Name), Constants.NotExistingSiteName, true, null)]
        [TestCase(nameof(Site.Name), "", false, ApplicationErrorCodes.SiteNameRequired)]
        [TestCase(nameof(Site.Name), null, false, ApplicationErrorCodes.SiteNameRequired)]
        [TestCase(nameof(Site.Description), "This is a test description", true, null)]
        [TestCase(nameof(Site.Description), "", true, null)]
        [TestCase(nameof(Site.Description), null, true, null)]
        [TestCase(nameof(Site.Country), "France", true, null)]
        [TestCase(nameof(Site.Country), "", false, ApplicationErrorCodes.SiteCountryRequired)]
        [TestCase(nameof(Site.Country), null, false, ApplicationErrorCodes.SiteCountryRequired)]
        [TestCase(nameof(Site.ZipCode), "6720", true, null)]
        [TestCase(nameof(Site.ZipCode), "", false, ApplicationErrorCodes.SiteZipCodeRequired)]
        [TestCase(nameof(Site.ZipCode), null, false, ApplicationErrorCodes.SiteZipCodeRequired)]
        [TestCase(nameof(Site.State), "Virginia", true, null)]
        [TestCase(nameof(Site.State), "", true, null)]
        [TestCase(nameof(Site.State), null, true, null)]
        [TestCase(nameof(Site.State), "Pest", true, null)]
        [TestCase(nameof(Site.State), "", true, null)]
        [TestCase(nameof(Site.State), null, true, null)]
        [TestCase(nameof(Site.Street), "Dugonics", true, null)]
        [TestCase(nameof(Site.Street), "", false, ApplicationErrorCodes.SiteStreetRequired)]
        [TestCase(nameof(Site.Street), null, false, ApplicationErrorCodes.SiteStreetRequired)]
        [TestCase(nameof(Site.HouseOrFlatNumber), "1/c", true, null)]
        [TestCase(nameof(Site.HouseOrFlatNumber), "", false, ApplicationErrorCodes.SiteHouseOrFlatNumberRequired)]
        [TestCase(nameof(Site.HouseOrFlatNumber), null, false, ApplicationErrorCodes.SiteHouseOrFlatNumberRequired)]
        public async Task CreateOrUpdateAsync_Test_CreateWithRequiredStringField(string fieldName, string value, bool operationShouldSucceed, string? expectedErrorCode)
        {
            var countOfSitesBeforeSave = await _repository.GetAll().CountAsync();

            var assertAction = () => _repository.CreateOrUpdateAsync(CreateSiteByField(fieldName, value));

            if (operationShouldSucceed)
            {
                await assertAction();
                var countOfSitesAfterSave = await _repository.GetAll().CountAsync();

                countOfSitesAfterSave.Should().Be(countOfSitesBeforeSave + 1);
            }
            else await assertAction.Should().ThrowExactlyAsync<DALException>().Where(e => e.ErrorCode == expectedErrorCode);
        }

        [TestCase(nameof(Site.Name), ApplicationConstants.SiteNameMaximiumLength, ApplicationErrorCodes.SiteNameTooLong)]
        [TestCase(nameof(Site.Description), ApplicationConstants.SiteDescriptionMaximiumLength, ApplicationErrorCodes.SiteDescriptionTooLong)]
        [TestCase(nameof(Site.Country), ApplicationConstants.SiteCountryMaximiumLength, ApplicationErrorCodes.SiteCountryTooLong)]
        [TestCase(nameof(Site.ZipCode), ApplicationConstants.SiteZipCodeMaximiumLength, ApplicationErrorCodes.SiteZipCodeTooLong)]
        [TestCase(nameof(Site.State), ApplicationConstants.SiteStateMaximiumLength, ApplicationErrorCodes.SiteStateTooLong)]
        [TestCase(nameof(Site.County), ApplicationConstants.SiteCountyMaximiumLength, ApplicationErrorCodes.SiteCountyTooLong)]
        [TestCase(nameof(Site.City), ApplicationConstants.SiteCityMaximiumLength, ApplicationErrorCodes.SiteCityTooLong)]
        [TestCase(nameof(Site.Street), ApplicationConstants.SiteStreetMaximiumLength, ApplicationErrorCodes.SiteStreetTooLong)]
        [TestCase(nameof(Site.HouseOrFlatNumber), ApplicationConstants.SiteHouseOrFlatNumberMaximiumLength, ApplicationErrorCodes.SiteHouseOrFlatNumberTooLong)]
        public async Task CreateOrUpdateAsync_Test_CreateWithMaxLengthStringField(string fieldName, int fieldMaximumLength, string? expectedErrorCode)
        {
            var assertAction = () => _repository.CreateOrUpdateAsync(CreateSiteByField(fieldName, new string('A', fieldMaximumLength + 1)));

            await assertAction.Should().ThrowExactlyAsync<DALException>().Where(e => e.ErrorCode == expectedErrorCode);
        }

        private Site CreateSiteByField(string fieldName, object? value) => fieldName switch
        {
            nameof(Site.Name) => CreateSite(name: (string?)value),
            nameof(Site.Description) => CreateSite(description: (string?)value),
            nameof(Site.Country) => CreateSite(country: (string?)value),
            nameof(Site.ZipCode) => CreateSite(zipCode: (string?)value),
            nameof(Site.State) => CreateSite(state: (string?)value),
            nameof(Site.County) => CreateSite(county: (string?)value),
            nameof(Site.City) => CreateSite(city: (string?)value),
            nameof(Site.Street) => CreateSite(street: (string?)value),
            nameof(Site.HouseOrFlatNumber) => CreateSite(houseOrFlatNumber: (string?)value),
            _ => throw new ArgumentException($"Field '{fieldName}' is not known on type '{nameof(Site)}'.")
        };

#pragma warning disable CS8601
        /// <summary>
        /// Creates a site with the configured properties. Pragma warning is used because cases with null property values are also tested regardless of the exact property type (nullable or not).
        /// The <paramref name="county"/> argument has a default value to avoid the acidentally violation of the database constraint <see cref="DatabaseConstraintNames.Site_StateCountry_CK"/>.
        /// </summary>
        /// <returns>A <see cref="Site"/> with the configured properties.</returns>
        private Site CreateSite(string? name = Constants.NotExistingSiteName, string? country = "Hungary", string? state = null, string? county = "Pest", string? city = "Budapest", string? street = "Hidden", string? houseOrFlatNumber = "1", string? zipCode = "9999", string? description = null, bool isDeleted = false) =>
            new Site { Name = name, Country = country, State = state, County = county, City = city, Street = street, HouseOrFlatNumber = houseOrFlatNumber, ZipCode = zipCode, Description = description, IsDeleted = isDeleted };
#pragma warning restore CS8601

    }
}
