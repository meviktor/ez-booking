using BookingWebAPI.DAL;
using Microsoft.EntityFrameworkCore.Design;
using Microsoft.EntityFrameworkCore;

namespace BookingWebAPI.Testing.Common
{
    /// <summary>
    /// This class is mainly for being able to create/add new migrations to the integration testing db schema, without being dependent 
    /// to the main web application (BookingWebAPI). To instantiate a DbContext, you need to resove its dependencies. There are options:
    /// - EF starts up the solutions start up project, as in there the DbContext's dependencies are probably resolved.
    /// - Using a factory class, which can create the DbContext independently from the startup project. This is that factory class.
    /// 
    /// Before adding a new migration, set this project as the startup project for the time of the operation. Also set this project as
    /// the default project in Package Manager Console.
    /// </summary>
    public class BookingWebAPIAzureSQLDbContextFactory : IDesignTimeDbContextFactory<BookingWebAPIAzureSQLDbContext>
    {
        /// <summary>
        /// Set this environment variable on your development machine only, if you want to try out (new) migrations locally (Update-Database command) 
        /// before running them in the Azure build pipeline. It has no effect to Azure build pipeline. 
        /// See <see cref="IntegrationTestBase.IntegrationTestBase"/> for case separation (running integration tests locally vs. in build pipeline).
        /// </summary>
        private const string EnvVarName = "ConnectionStrings__TestingAzureSQLDbContextFactory";

        public BookingWebAPIAzureSQLDbContext CreateDbContext(string[] args)
        {
            var optionsBuilder = new DbContextOptionsBuilder<BookingWebAPIDbContext>();
            optionsBuilder.UseSqlServer(
                // EnvVarName does not need to be set, if this factory class is involved only adding a new migration (Add-Migration command).
                // In case of trying out the new migration locally (Update-Database command), set the environment variable to a valid connection string.
                Environment.GetEnvironmentVariable(EnvVarName) ?? string.Empty,
                o => o.MigrationsHistoryTable("__EFMigrationsHistory", "dbo_testing"));
            return new BookingWebAPIAzureSQLDbContext(optionsBuilder.Options);
        }
    }
}
