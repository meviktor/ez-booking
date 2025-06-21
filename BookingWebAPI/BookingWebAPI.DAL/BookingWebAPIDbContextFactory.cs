using Microsoft.EntityFrameworkCore.Design;
using Microsoft.EntityFrameworkCore;

namespace BookingWebAPI.DAL
{
    /// <summary>
    /// This class is mainly for being able to create/add new migrations to the application's db schema, without being dependent 
    /// to the main web application (BookingWebAPI). To instantiate a DbContext, you need to resove its dependencies. There are options:
    /// - EF starts up the solutions start up project, as in there the DbContext's dependencies are probably resolved.
    /// - Using a factory class, which can create the DbContext independently from the startup project. This is that factory class.
    /// 
    /// Before adding a new migration, set this project as the startup project for the time of the operation. Also set this project as
    /// the default project in Package Manager Console.
    /// </summary>
    public class BookingWebAPIDbContextFactory : IDesignTimeDbContextFactory<BookingWebAPIDbContext>
    {
        /// <summary>
        /// This environment varibale has to be set on your local dev machine. The connection string should point of your local SQL Server database.
        /// </summary>
        private const string EnvVarName = "ConnectionStrings__DbContextFactory";

        public BookingWebAPIDbContext CreateDbContext(string[] args)
        {
            var connectionString = Environment.GetEnvironmentVariable(EnvVarName);
            if (connectionString != null)
            {
                return new BookingWebAPIDbContext(new DbContextOptionsBuilder<BookingWebAPIDbContext>().UseSqlServer(connectionString).Options);
            }

            throw new InvalidOperationException($"No connection string has been found. Check the value of the environment variable '{EnvVarName}'.");
        }
    }
}
