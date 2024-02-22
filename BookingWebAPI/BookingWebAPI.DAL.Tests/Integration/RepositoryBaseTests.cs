using BookingWebAPI.Common.Interfaces;
using BookingWebAPI.Common.Models;
using BookingWebAPI.DAL.Repositories;
using BookingWebAPI.Testing.Common;
using FluentAssertions;
using Microsoft.EntityFrameworkCore;
using NUnit.Framework;
using static BookingWebAPI.Testing.Common.TestDatabaseSeeder;

namespace BookingWebAPI.DAL.Tests.Integration
{
    internal class RepositoryBaseTests : IntegrationTestBase
    {
        private TestRepositoryBase<BookingWebAPIUser> _repository;

        [SetUp]
        public override void SetUp()
        {
            base.SetUp();
            _repository = new TestRepositoryBase<BookingWebAPIUser>(_dbContext);
        }

        [TestCase(Constants.ActiveUserId, true)]
        [TestCase(Constants.DeletedUserId, true)]
        [TestCase(null, false)]
        public async Task GetAsync_Test(string? userId, bool shouldFoundUser)
        {
            // action
            var foundUser = await _repository.GetAsync(userId != null ? Guid.Parse(userId) : Guid.Empty);

            // assert
            if (shouldFoundUser)
            {
                foundUser.Should().NotBeNull();
                foundUser?.Id.Should().Be(userId);
            }
            else
            {
                foundUser.Should().BeNull();
            }
        }

        [TestCase(Constants.ActiveUserId, true)]
        [TestCase(Constants.DeletedUserId, true)]
        [TestCase(null, false)]
        public async Task ExistsAsync_Test(string? userId, bool shouldFoundUser)
        {
            // action
            var userFound = await _repository.ExistsAsync(userId != null ? Guid.Parse(userId) : Guid.Empty);

            // assert
            userFound.Should().Be(shouldFoundUser);
        }

        [Test]
        public async Task GetAll_Test()
        {
            // prepare
            var queryable = _repository.GetAll();

            // action
            var queryableCount = await queryable.CountAsync();

            // assert
            var setCount = await _dbContext.Users.CountAsync();
            queryableCount.Should().Be(setCount);
        }
    }

    internal class TestRepositoryBase<T> : RepositoryBase<T> where T : class, IEntity
    {
        internal TestRepositoryBase(BookingWebAPIDbContext ctx) : base(ctx) { }
    }
}
