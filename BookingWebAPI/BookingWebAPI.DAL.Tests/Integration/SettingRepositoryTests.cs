using BookingWebAPI.Common.Enums;
using BookingWebAPI.DAL.Repositories;
using BookingWebAPI.Testing.Common;
using FluentAssertions;
using NUnit.Framework;

namespace BookingWebAPI.DAL.Tests.Integration
{
    internal class SettingRepositoryTests : IntegrationTestBase
    {
        private SettingRepository _repository;

        [SetUp]
        public override void SetUp()
        {
            base.SetUp();
            _repository = new SettingRepository(_dbContext);
        }

        [TestCase(null, false)]
        [TestCase("", false)]
        [TestCase(TestDatabaseSeeder.Constants.ActiveSettingName, true)]
        [TestCase(TestDatabaseSeeder.Constants.DeletedSettingName, false)]
        public async Task GetSettingByNameAsync_Test(string settingName, bool resultsExpected)
        {
            // action
            var foundSetting = await _repository.GetSettingByNameAsync(settingName);

            // assert
            if (resultsExpected)
            {
                foundSetting?.Should().NotBeNull();
                foundSetting?.Id.Should().Be(Guid.Parse(TestDatabaseSeeder.Constants.ActiveSettingId));
                foundSetting?.Name.Should().Be(TestDatabaseSeeder.Constants.ActiveSettingName);
            }
            else foundSetting.Should().BeNull();
        }

        [TestCase((SettingCategory)0, false)]
        [TestCase(TestDatabaseSeeder.Constants.SettingCategoryTesting, true)]
        [TestCase(SettingCategory.PasswordPolicy, false)]
        public async Task GetSettingsForCategoryAsync_Test(SettingCategory settingCategory, bool resultsExpected)
        {
            // action
            var foundSettings = await _repository.GetSettingsForCategoryAsync(settingCategory);

            // assert
            if (resultsExpected)
            {
                foundSettings.Should().NotBeEmpty();
                foundSettings.All(s => !s.IsDeleted).Should().BeTrue();
                foundSettings.All(s => s.Category == settingCategory).Should().BeTrue();
            }
            else foundSettings.Should().BeEmpty();
        }
    }
}
