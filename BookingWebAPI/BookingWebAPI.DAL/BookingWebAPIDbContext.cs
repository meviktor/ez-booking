using BookingWebAPI.Common.Models;
using BookingWebAPI.Common.Constants;
using Microsoft.EntityFrameworkCore;

namespace BookingWebAPI.DAL
{
    public class BookingWebAPIDbContext : DbContext
    {
        public BookingWebAPIDbContext(DbContextOptions<BookingWebAPIDbContext> options)
            : base(options)
        {}

        public DbSet<Resource> Resources { get; set; } = null!;
        public DbSet<ResourceCategory> ResourceCategories { get; set; } = null!;
        public DbSet<Site> Sites { get; set; } = null!;
        public DbSet<BookingWebAPIUser> Users { get; set; } = null!;

        protected override void OnModelCreating(ModelBuilder builder)
        {
            base.OnModelCreating(builder);
            builder.Entity<Site>().ToTable(site => site.HasCheckConstraint(DatabaseConstraintNames.Site_StateCountry_CK, $"COALESCE({nameof(Site.State)}, {nameof(Site.County)}) IS NOT NULL"));
        }
    }
}
