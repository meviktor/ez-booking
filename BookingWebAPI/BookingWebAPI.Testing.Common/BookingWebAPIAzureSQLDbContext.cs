using BookingWebAPI.DAL;
using Microsoft.EntityFrameworkCore;

namespace BookingWebAPI.Testing.Common
{
    /// <summary>
    /// Only for testing in the build pipeline! The differentiation of the database providers is only meaningful in integration testing!
    /// Database context class for conducting the integration testing under the same database as the demo application uses, but under its own schema.
    /// </summary>
    public class BookingWebAPIAzureSQLDbContext : BookingWebAPIDbContext
    {
        public BookingWebAPIAzureSQLDbContext(DbContextOptions<BookingWebAPIDbContext> options) : base(options) {}

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            // Creating the model in the designated schema for testing
            modelBuilder.HasDefaultSchema("dbo_testing");
            base.OnModelCreating(modelBuilder);
        }
    }
}
