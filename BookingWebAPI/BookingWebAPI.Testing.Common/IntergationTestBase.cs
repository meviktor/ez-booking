using BookingWebAPI.DAL;
using BookingWebAPI.DAL.Infrastructure;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Storage;
using Microsoft.Extensions.Configuration;

namespace BookingWebAPI.Testing.Common
{

    public class IntegrationTestBase
    {
        enum DbProvider { LocalSQLServer, RemoteAzureSQL };

        private const string DatabaseProvider_LocalSQLServer = "LocalSQLServer";
        private const string DatabaseProvider_RemoteAzureSQL = "RemoteAzureSQL";

        private const string Error_DatabaseInitFailed = "Could not intialize the database with necessary records. See the inner exception for more details.";
        private const string Error_DatabaseRecreateFailed = "Could not recreate the database.";
        private const string Error_NoConnectionString = "Could not found database connection string.";
        private const string Error_UnknownDatabaseProvider = "Database provider '{0}' is not known. Check the value of the 'DatabaseProvider' environment variable.";
        private const string Error_AzureSQLMigrationFailed = "Migration in AzureSQL database failed. See the inner exception for more details.";


        //private static readonly IConfiguration _testConfiguration;
        private static readonly object _lock = new();
        /// <summary>
        /// Applicable only if tests run against a persistent database, not an in-memory one.
        /// </summary>
        private static bool _databaseInitialized;

        private static readonly DbProvider _databaseProvider;
        private static readonly string _connectionString = null!;

        protected BookingWebAPIDbContext _dbContext;
        protected IDbContextTransactionManager _transactionManager;

        static IntegrationTestBase()
        {
            var envVarDbProvider = Environment.GetEnvironmentVariable("DatabaseProvider") ?? DatabaseProvider_LocalSQLServer;
            _databaseProvider = envVarDbProvider switch
            {
                DatabaseProvider_LocalSQLServer => DbProvider.LocalSQLServer,
                DatabaseProvider_RemoteAzureSQL => DbProvider.RemoteAzureSQL,
                _ => throw new IntegrationTestSetupException(string.Format(Error_UnknownDatabaseProvider, envVarDbProvider))
            };

            _connectionString =
                    (_databaseProvider == DbProvider.LocalSQLServer ?
                    new ConfigurationBuilder().AddJsonFile("appsettings.json").Build()["TestDbConnection"] :
                    Environment.GetEnvironmentVariable("TestDbConnection"))
                 ?? 
                 throw new IntegrationTestSetupException(Error_NoConnectionString);
        }

        protected IntegrationTestBase()
        {
            lock (_lock)
            {
                if (!_databaseInitialized)
                {
                    using (var context = GetDatabaseContext())
                    {
                        if (_databaseProvider == DbProvider.LocalSQLServer)
                        {
                            bool isDbRecreated = context.Database.EnsureDeleted() && context.Database.EnsureCreated();
                            if (!isDbRecreated)
                            {
                                throw new IntegrationTestSetupException(Error_DatabaseRecreateFailed);
                            }
                        }
                        else // for AzureSQL db - migrating instead of drop & recreate
                        {
                            try
                            {
                                context.Database.Migrate();
                            }
                            catch (Exception e)
                            {
                                throw new IntegrationTestSetupException(Error_AzureSQLMigrationFailed, e);
                            }
                        }

                        try
                        {
                            context.SeedTestData(_databaseProvider == DbProvider.LocalSQLServer);
                        }
                        catch (Exception e)
                        {
                            throw new IntegrationTestSetupException(Error_DatabaseInitFailed, e);
                        }
                    }
                    _databaseInitialized = true;
                }
            }
        }

        private BookingWebAPIDbContext GetDatabaseContext() => _databaseProvider == DbProvider.LocalSQLServer ?
            new BookingWebAPIDbContext(new DbContextOptionsBuilder<BookingWebAPIDbContext>().UseSqlServer(_connectionString).Options) :
            new BookingWebAPIAzureSQLDbContext(new DbContextOptionsBuilder<BookingWebAPIDbContext>()
                 .UseSqlServer(_connectionString, o => o.MigrationsHistoryTable("__EFMigrationsHistory", "dbo_testing"))
                 .Options);

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
