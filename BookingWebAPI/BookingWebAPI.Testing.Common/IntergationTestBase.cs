using BookingWebAPI.DAL;
using BookingWebAPI.DAL.Infrastructure;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Storage;
using Microsoft.Extensions.Configuration;

namespace BookingWebAPI.Testing.Common
{
    public class IntegrationTestBase
    {
        private static readonly IConfiguration _testConfiguration;
        private static readonly object _lock = new();
        private static bool _databaseInitialized;

        private static string? ConnectionString => _testConfiguration["TestDbConnection"];

        protected BookingWebAPIDbContext _dbContext;
        protected IDbContextTransactionManager _transactionManager;

        static IntegrationTestBase()
        {
            _testConfiguration = new ConfigurationBuilder().AddJsonFile("appsettings.json").Build();
        }

        protected IntegrationTestBase()
        {
            lock (_lock)
            {
                if (!_databaseInitialized)
                {
                    using (var context = GetDatabaseContext())
                    {
                        context.Database.EnsureDeleted();
                        context.Database.EnsureCreated();
                        context.SeedTestData();
                    }
                    _databaseInitialized = true;
                }
            }
        }

        protected static BookingWebAPIDbContext GetDatabaseContext() => 
            new BookingWebAPIDbContext(new DbContextOptionsBuilder<BookingWebAPIDbContext>().UseSqlServer(ConnectionString).Options);

        [SetUp]
        public virtual void SetUp()
        {
            _dbContext = GetDatabaseContext();
            _transactionManager = new BookingWebAPITransactionManager(_dbContext);

            _transactionManager.BeginTransaction();
        }

        [TearDown]
        public virtual void TearDown()
        {
            _transactionManager.RollbackTransaction();
            _dbContext.Dispose();
        }
    }
}