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
    internal class ResourceCategoryRepositoryTests : IntegrationTestBase
    {
        private ResourceCategoryRepository _repository;

        [SetUp]
        public override void SetUp()
        {
            base.SetUp();
            _repository = new ResourceCategoryRepository(_dbContext);
        }

        [Test]
        public async Task CreateOrUpdateAsync_Test()
        {
            // prepare
            var countOfResourceCategoriesBeforeSave = await _repository.GetAll().CountAsync();

            // action
            await _repository.CreateOrUpdateAsync(CreateResourceCategoryByField());

            // assert
            var countOfResourceCategoriesAfterSave = await _repository.GetAll().CountAsync();
            countOfResourceCategoriesAfterSave.Should().Be(countOfResourceCategoriesBeforeSave + 1);
        }

        [Test]
        public async Task CreateOrUpdateAsync_Test_NameDuplicate()
        {
            var action = async () => await _repository.CreateOrUpdateAsync(CreateResourceCategoryByField());

            // prepare: insert resource category with the default name
            await action();
            var countOfResourceCategoriesBeforeAttempt = await _repository.GetAll().CountAsync();

            // action & assert: inserting resource category with the default name again
            await action.Should().ThrowExactlyAsync<DALException>().Where(e => e.ErrorCode == ApplicationErrorCodes.ResourceCategoryNameMustBeUnique);

            var countOfResourceCategoriesAfterAttempt = await _repository.GetAll().CountAsync();
            countOfResourceCategoriesAfterAttempt.Should().Be(countOfResourceCategoriesBeforeAttempt);
        }

        [TestCase(null, ApplicationErrorCodes.ResourceCategoryNameRequired)]
        [TestCase(151, ApplicationErrorCodes.ResourceCategoryNameTooLong)] // current name length limit + 1
        public async Task CreateOrUpdateAsync_Test_InvalidName(int? nameLength, string expectedErrorMessage)
        {
            var invalidName = nameLength != null ? new string('A', nameLength.Value) : null;
            var action = async () => await _repository.CreateOrUpdateAsync(CreateResourceCategoryByField(name: invalidName));

            // prepare
            var countOfResourceCategoriesBeforeAttempt = await _repository.GetAll().CountAsync();

            // action & assert
            await action.Should().ThrowExactlyAsync<DALException>().Where(e => e.ErrorCode == expectedErrorMessage);

            var countOfResourceCategoriesAfterAttempt = await _repository.GetAll().CountAsync();
            countOfResourceCategoriesAfterAttempt.Should().Be(countOfResourceCategoriesBeforeAttempt);
        }

        [Test]
        public async Task CreateOrUpdateAsync_Test_DescriptionTooLong()
        {
            var tooLongDescription = new string('A', 1001); // current description length limit + 1
            var action = async () => await _repository.CreateOrUpdateAsync(CreateResourceCategoryByField(description: tooLongDescription));

            // prepare
            var countOfResourceCategoriesBeforeAttempt = await _repository.GetAll().CountAsync();

            // action & assert
            await action.Should().ThrowExactlyAsync<DALException>().Where(e => e.ErrorCode == ApplicationErrorCodes.ResourceCategoryDescriptionTooLong);

            var countOfResourceCategoriesAfterAttempt = await _repository.GetAll().CountAsync();
            countOfResourceCategoriesAfterAttempt.Should().Be(countOfResourceCategoriesBeforeAttempt);
        }

#pragma warning disable CS8601
        private static ResourceCategory CreateResourceCategoryByField(string? name = "DefaultName", string? description = "DefaultDescription", string? baseCategoryId = null)
            => new ResourceCategory { Name = name, Description = description, BaseCategoryId = baseCategoryId != null ? Guid.Parse(baseCategoryId) : null };
#pragma warning restore CS8601
    }
}
