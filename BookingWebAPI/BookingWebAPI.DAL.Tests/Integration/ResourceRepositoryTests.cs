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
    internal class ResourceRepositoryTests : IntegrationTestBase
    {
        private ResourceRepository _repository;

        [SetUp]
        public override void SetUp()
        {
            base.SetUp();
            _repository = new ResourceRepository(_dbContext);
        }
       
        [Test]
        public async Task CreateOrUpdateAsync_Test()
        {
            // prepare
            var countOfResourcesBeforeSave = await _repository.GetAll().CountAsync();

            // action
            await _repository.CreateOrUpdateAsync(CreateResourceByField());

            // assert
            var countOfResourcesAfterSave = await _repository.GetAll().CountAsync();
            countOfResourcesAfterSave.Should().Be(countOfResourcesBeforeSave + 1);
        }

        [Test]
        public async Task CreateOrUpdateAsync_Test_NameDuplicate()
        {
            var action = async () => await _repository.CreateOrUpdateAsync(CreateResourceByField());

            // prepare: insert resource with the default name
            await action();
            var countOfResourcesBeforeAttempt = await _repository.GetAll().CountAsync();

            // action & assert: inserting resource with the default name again
            await action.Should().ThrowExactlyAsync<DALException>().Where(e => e.ErrorCode == ApplicationErrorCodes.ResourceNameMustBeUnique);

            var countOfResourcesAfterAttempt = await _repository.GetAll().CountAsync();
            countOfResourcesAfterAttempt.Should().Be(countOfResourcesBeforeAttempt);
        }

        [TestCase(null, ApplicationErrorCodes.ResourceNameRequired)]
        [TestCase(151, ApplicationErrorCodes.ResourceNameTooLong)] // current name length limit + 1
        public async Task CreateOrUpdateAsync_Test_InvalidName(int? nameLength, string expectedErrorMessage)
        {
            var invalidName = nameLength != null ? new string('A', nameLength.Value) : null;
            var action = async () => await _repository.CreateOrUpdateAsync(CreateResourceByField(name: invalidName));

            // prepare
            var countOfResourcesBeforeAttempt = await _repository.GetAll().CountAsync();

            // action & assert
            await action.Should().ThrowExactlyAsync<DALException>().Where(e => e.ErrorCode == expectedErrorMessage);

            var countOfResourcesAfterAttempt = await _repository.GetAll().CountAsync();
            countOfResourcesAfterAttempt.Should().Be(countOfResourcesBeforeAttempt);
        }

        [Test]
        public async Task CreateOrUpdateAsync_Test_DescriptionTooLong()
        {
            var tooLongDescription = new string('A', 1001); // current description length limit + 1
            var action = async () => await _repository.CreateOrUpdateAsync(CreateResourceByField(description: tooLongDescription));

            // prepare
            var countOfResourcesBeforeAttempt = await _repository.GetAll().CountAsync();

            // action & assert
            await action.Should().ThrowExactlyAsync<DALException>().Where(e => e.ErrorCode == ApplicationErrorCodes.ResourceDescriptionTooLong);

            var countOfResourcesAfterAttempt = await _repository.GetAll().CountAsync();
            countOfResourcesAfterAttempt.Should().Be(countOfResourcesBeforeAttempt);
        }

#pragma warning disable CS8601
        private static Resource CreateResourceByField(string? name = "DefaultName", string? description = "DefaultDescription", string resourceCategoryId = TestDatabaseSeeder.Constants.ActiveResourceCategoryId, string siteId = TestDatabaseSeeder.Constants.ActiveSiteId)
            => new Resource { Name = name, Description = description, ResourceCategoryId = Guid.Parse(resourceCategoryId), SiteId = Guid.Parse(siteId) };
#pragma warning restore CS8601
    }
}
