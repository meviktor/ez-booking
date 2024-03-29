using BookingWebAPI.Common.Models;
using BookingWebAPI.Common.Models.Config;
using BookingWebAPI.DAL;
using BookingWebAPI.DAL.Interfaces;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Infrastructure;
using Microsoft.EntityFrameworkCore.Storage;
using Microsoft.Extensions.Options;
using Moq;
using Moq.EntityFrameworkCore;
using System.Reflection;

namespace BookingWebAPI.Testing.Common
{
    public class UnitTestBase
    {
        private Mock<IOptions<JwtConfiguration>> _jwtMock;
        private Mock<BookingWebAPIDbContext> _dbContextMock;
        private Mock<DatabaseFacade> _databaseMock;
        private Mock<IDbContextTransactionManager> _transactionManagerMock;
        private Mock<IDbContextTransaction> _transactionMock;

        public IOptions<JwtConfiguration> JwtConfigMock => _jwtMock.Object;
        public BookingWebAPIDbContext DbContextMock => _dbContextMock.Object;
        public IDbContextTransactionManager TransactionManagerMock => _transactionManagerMock.Object;

        public UnitTestBase()
        {
            _jwtMock = new Mock<IOptions<JwtConfiguration>>();
            _dbContextMock = new Mock<BookingWebAPIDbContext>(new DbContextOptions<BookingWebAPIDbContext>());
            _databaseMock = new Mock<DatabaseFacade>(_dbContextMock.Object);
            _transactionManagerMock = new Mock<IDbContextTransactionManager>();
            _transactionMock = new Mock<IDbContextTransaction>();

            _jwtMock.Setup(jwtConfig => jwtConfig.Value).Returns(new JwtConfiguration { Secret = "YA1o0Bo1FEH4HedoYA1o0Bo1FEH4Hedo", ValidInSeconds = 3600});
            _transactionManagerMock.Setup(db => db.BeginTransaction()).Returns(_transactionMock.Object);
            _dbContextMock.Setup(ctx => ctx.Database).Returns(_databaseMock.Object);
            SetupDbContextMockDbSets();
        }

        /// <summary>
        /// Registering every generic Set<T>() call to return a DbSet mock, which returns the matching collection from <see cref="MockDbCollections"/> by type.
        /// For example: a Set<Site>() on the mock DbContext returns a DbSet<Site> mock, which returns the public <see cref="IList<Site>"/> list from the <see cref="MockDbCollections"/> static class (Sites).
        /// </summary>
        private void SetupDbContextMockDbSets()
        {
            var dbSetProperties = typeof(BookingWebAPIDbContext).GetProperties().Where(p => p.PropertyType.IsGenericType && p.PropertyType.GetGenericTypeDefinition().Equals(typeof(DbSet<>)));
            var mockListProperties = typeof(MockDbCollections).GetProperties().Where(p => p.PropertyType.IsGenericType && p.PropertyType.GetGenericTypeDefinition().Equals(typeof(IList<>)));

            foreach (var dbSetProperty in dbSetProperties)
            {
                var modelType = dbSetProperty.PropertyType.GenericTypeArguments.Single();
                var mockListProperty = mockListProperties.SingleOrDefault(mp => mp.PropertyType.GenericTypeArguments.Single().Equals(modelType));

                // If we could not find a matching IList collection from MockDbCollections to the current DbSet by type match, then we skip setting a mock DbSet with this entity type to the mock DbContext.
                // Eventually only for those entity types will be the Set<T> call configured, which have a matching collection in the MockDbCollections class.
                if (mockListProperty != null)
                {
                    // As we define tests in descendant classes of UnitTestBase, we have to Reference this class by the BaseType property to access its private MockDbSetWithList method.
                    var mockMethod = GetType().BaseType?.GetMethod(nameof(MockDbSetWithList), BindingFlags.NonPublic | BindingFlags.Instance);
                    mockMethod?.MakeGenericMethod(modelType).Invoke(this, new object?[] { mockListProperty.GetValue(null) });
                }
            }
        }

        /// <summary>
        /// Adds a mock DbSet to the mock DbContext.
        /// </summary>
        /// <typeparam name="T">Entity type of the DbSet.</typeparam>
        /// <param name="listToReturn">The list which will be returned by the fake DbSet.</param>
        private void MockDbSetWithList<T>(IList<T> listToReturn) where T : class => _dbContextMock.Setup(db => db.Set<T>()).ReturnsDbSet(listToReturn);
    }

    static class MockDbCollections
    {
        private static IList<Site> _sites = new List<Site>();
        private static IList<BookingWebAPIUser> _users = new List<BookingWebAPIUser>();

        public static IList<Site> Sites => _sites;
        public static IList<BookingWebAPIUser> Users => _users;

        static MockDbCollections()
        {
            _sites.Add(new Site { Id = Guid.NewGuid(), Name = "TestSite" });
            _users.Add(new BookingWebAPIUser { Email = "testuser@ezbooking.com", FirstName = "Test", LastName = "User", EmailConfirmed = true, UserName = "testUser", SiteId = _sites.First().Id, Site = _sites.First() });
        }
    }
}
