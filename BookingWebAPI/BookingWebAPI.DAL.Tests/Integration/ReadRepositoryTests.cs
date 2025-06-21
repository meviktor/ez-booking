using BookingWebAPI.Common.Models;
using BookingWebAPI.DAL.Infrastructure;
using BookingWebAPI.DAL.Repositories;
using BookingWebAPI.Testing.Common;
using FluentAssertions;
using Microsoft.EntityFrameworkCore;
using NUnit.Framework;

namespace BookingWebAPI.DAL.Tests.Integration
{
    /// <summary>
    /// Class created for testing ReadRepository<T>, as it cannot be instantiated.
    /// </summary>
    /// <typeparam name="T"></typeparam>
    internal class ReadRepositoryDerived<T> : ReadRepository<T> where T : ModelBase 
    {
        public ReadRepositoryDerived(BookingWebAPIDbContext dbContext) : base(dbContext)
        {
        }

        public override IEnumerable<ErrorCodeAssociation> ErrorCodeAssosications => new ErrorCodeAssociation[] { };
 
    }

    [TestFixture]
    internal class ReadRepositoryTests : IntegrationTestBase
    {
        private ReadRepositoryDerived<Site> _repository;

        [SetUp]
        public override void SetUp()
        {
            base.SetUp();
            _repository = new ReadRepositoryDerived<Site>(_dbContext);
        }

        [Test]
        public async Task GetAsync_Test_CallWithExistingId()
        {
            var activeSiteId = new Guid(TestDatabaseSeeder.Constants.ActiveSiteId);

            var retrievedSite = await _repository.GetAsync(activeSiteId);

            retrievedSite?.Id.Should().Be(activeSiteId);
        }

        [TestCase(TestDatabaseSeeder.Constants.EmptyId)]
        [TestCase(TestDatabaseSeeder.Constants.DeletedSiteId)]
        public async Task GetAsync_Test_CallWithNotExistingId(string idAsString)
        {
            var action = async () => await _repository.GetAsync(new Guid(idAsString));

            (await action()).Should().BeNull();
        }

        [Test]
        public async Task ExistsAsync_Test_CallWithExistingId()
        {
            var siteExists = await _repository.ExistsAsync(new Guid(TestDatabaseSeeder.Constants.ActiveSiteId));

            siteExists.Should().BeTrue();
        }

        [TestCase(TestDatabaseSeeder.Constants.EmptyId)]
        [TestCase(TestDatabaseSeeder.Constants.DeletedSiteId)]
        public async Task ExistsAsync_Test_CallWithNotExistingId(string idAsString)
        {
            var siteExists = await _repository.ExistsAsync(new Guid(idAsString));

            siteExists.Should().BeFalse();
        }

        [Test]
        public async Task GetAll_Test()
        {
            var itemsNotDeleted = TestDatabaseSeeder.Sites.Count(s => !s.IsDeleted);

            var itemsRetrieved = await _repository.GetAll().CountAsync();

            itemsRetrieved.Should().Be(itemsNotDeleted);
        }
    }
}
