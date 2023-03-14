using BookingWebAPI.Common.Models;
using BookingWebAPI.Common.Constants;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.ChangeTracking;

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

        /// <summary>
        /// Overrides the default SaveChanges() implementation with an additional step which converts empty strings or strings containing only whitespace characters to null.
        /// </summary>
        /// <returns>The number ofn state entries written to the database.</returns>
        public override int SaveChanges()
        {
            var entriesToSave = ChangeTracker.Entries().Where(e => e.State == EntityState.Added || e.State == EntityState.Modified);
            foreach (var entry in entriesToSave)
            {
                var stringFieldsOfEntry = entry.Members
                    .Where(m => m.Metadata.FieldInfo != null && m.Metadata.FieldInfo.FieldType.Name == typeof(string).Name)
                    .Select(m => m.Metadata.FieldInfo!.Name);
                foreach(var stringField in stringFieldsOfEntry)
                {
                    var property = entry.Property(stringField);
                    property.CurrentValue = !string.IsNullOrWhiteSpace((string?)property.CurrentValue) ? property.CurrentValue : null;
                }
            }
            return base.SaveChanges();
        }
    }
}
