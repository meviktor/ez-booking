﻿using BookingWebAPI.Common.Models;
using BookingWebAPI.DAL.Repositories;
using BookingWebAPI.Testing.Common;
using NUnit.Framework;
using FluentAssertions;
using Microsoft.EntityFrameworkCore;
using BookingWebAPI.Common.Exceptions;
using BookingWebAPI.Common.ErrorCodes;

namespace BookingWebAPI.DAL.Tests.Integration
{
    internal class ReadRepositoryTests : IntegrationTestBase
    {
        private ReadRepository<Site> _repository;

        [SetUp]
        public override void SetUp()
        {
            base.SetUp();
            _repository = new ReadRepository<Site>(_dbContext);
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
