using BookingWebAPI.Common.Models;
using BookingWebAPI.Common.Constants;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.ChangeTracking;
using BookingWebAPI.Common.Enums;

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
        public DbSet<BookingWebAPISetting> Settings { get; set; } = null!;

        protected override void OnModelCreating(ModelBuilder builder)
        {
            base.OnModelCreating(builder);
            builder.Entity<Site>().ToTable(site => site.HasCheckConstraint(DatabaseConstraintNames.Site_StateCountry_CK, $"COALESCE({nameof(Site.State)}, {nameof(Site.County)}) IS NOT NULL"));
        }

        /// <summary>
        /// Its intended use is development, it is intended to be called only in development mode, at application startup. DO NOT use it in production scenarios.
        /// </summary>
        public void Seed()
        {
            using var transaction = Database.BeginTransaction();
            if(!Sites.Any())
            {
                var site = new Site { Id = Guid.NewGuid(), City = "Szeged", Country = "Hungary", ZipCode = "6720", County = "Csongrad", Street = "Dugonics", HouseOrFlatNumber = "13", Name = "Szeged site", Description = "HQ" };
                Sites.Add(site);

                SaveChanges();
            }

            if(!Users.Any())
            {
                // admin@BookingWebAPI1
                var user = new BookingWebAPIUser { Id = Guid.NewGuid(), Email = "admin@bookingwebapi.com", EmailConfirmed = true, FirstName = "Admin", LastName = "User", Site = Sites.First(), UserName = "superadmin", PasswordHash = "$2y$11$JCJPHetWFVCMSuxgq7FT2.3Oi1sdt60ydeDt87JiX/0981I/jYO86" };
                Users.Add(user);
            }

            if (!ResourceCategories.Any())
            {
                var resourceCategoryRooms = new ResourceCategory { Id = Guid.NewGuid(), Name = "Rooms", Description = "Resource category containing all kinds of available rooms.", BaseCategory = null };
                var resourceCategoryMeetingRooms = new ResourceCategory { Id = Guid.NewGuid(), Name = "Meeting rooms", Description = "Available rooms for planning meetings.", BaseCategory = resourceCategoryRooms };
                var resource = new Resource { Id = Guid.NewGuid(), Name = "Meeting room 1st floor", Description = "Meeting room for short meetings.", ResourceCategory = resourceCategoryMeetingRooms, Site = Sites.First() };

                ResourceCategories.AddRange(new[] { resourceCategoryRooms, resourceCategoryMeetingRooms });
                Resources.Add(resource);
            }

            if(!Settings.Any())
            {
                Settings.AddRange(new[]
                {
                    new BookingWebAPISetting { Id = Guid.NewGuid(), Category = SettingCategory.PasswordPolicy, Name = ApplicationConstants.PasswordPolicyMinLength, ValueType = SettingValueType.Integer, RawValue = "8" },
                    new BookingWebAPISetting { Id = Guid.NewGuid(), Category = SettingCategory.PasswordPolicy, Name = ApplicationConstants.PasswordPolicyMaxLength, ValueType = SettingValueType.Integer, RawValue = "500" },
                    new BookingWebAPISetting { Id = Guid.NewGuid(), Category = SettingCategory.PasswordPolicy, Name = ApplicationConstants.PasswordPolicyUppercaseLetter, ValueType = SettingValueType.Boolean, RawValue = "false" },
                    new BookingWebAPISetting { Id = Guid.NewGuid(), Category = SettingCategory.PasswordPolicy, Name = ApplicationConstants.PasswordPolicySpecialCharacters, ValueType = SettingValueType.Boolean, RawValue = "false" },
                    new BookingWebAPISetting { Id = Guid.NewGuid(), Category = SettingCategory.PasswordPolicy, Name = ApplicationConstants.PasswordPolicyDigits, ValueType = SettingValueType.Boolean, RawValue = "false" },
                    new BookingWebAPISetting { Id = Guid.NewGuid(), Category = SettingCategory.Email, Name = ApplicationConstants.UserRegistrationConfirmationEmailSubject, ValueType = SettingValueType.String, RawValue = ApplicationConstants.UserRegistrationConfirmationEmailDefaultSubject },
                    new BookingWebAPISetting { Id = Guid.NewGuid(), Category = SettingCategory.Email, Name = ApplicationConstants.UserRegistrationConfirmationEmailContent, ValueType = SettingValueType.String, RawValue = ApplicationConstants.UserRegistrationConfirmationEmailDefaultContent },
                    // Email settings: provide the e-mail settings here for development/testing purpose
                    //new BookingWebAPISetting { Id = Guid.NewGuid(), Category = SettingCategory.Email, Name = ApplicationConstants.EmailSmtpServer, ValueType = SettingValueType.String, RawValue = string.Empty },
                    //new BookingWebAPISetting { Id = Guid.NewGuid(), Category = SettingCategory.Email, Name = ApplicationConstants.EmailSmtpPort, ValueType = SettingValueType.Integer, RawValue = "0" },
                    //new BookingWebAPISetting { Id = Guid.NewGuid(), Category = SettingCategory.Email, Name = ApplicationConstants.EmailSenderEmailAddress, ValueType = SettingValueType.String, RawValue = string.Empty }
                });
            }

            SaveChanges();
            transaction.Commit();
            // Exceptions will be caught in the application startup logic from where Seed() is called.
        }

        /// <summary>
        /// Overrides the default SaveChanges() implementation with an additional step which converts empty strings or strings containing only whitespace characters to null.
        /// </summary>
        /// <returns>The number of state entries written to the database.</returns>
        public override int SaveChanges()
        {
            ConvertWhiteSpaceToNull();
            return base.SaveChanges();
        }

        /// <summary>
        /// Overrides the default SaveChanges() implementation with an additional step which converts empty strings or strings containing only whitespace characters to null.
        /// </summary>
        /// <returns>The number of state entries written to the database.</returns>
        public async override Task<int> SaveChangesAsync(CancellationToken cancellationToken = default)
        {
            ConvertWhiteSpaceToNull();
            return await base.SaveChangesAsync(cancellationToken);
        }

        private void ConvertWhiteSpaceToNull()
        {
            var entriesToSave = ChangeTracker.Entries().Where(e => e.State == EntityState.Added || e.State == EntityState.Modified);
            foreach (var entry in entriesToSave)
            {
                var stringFieldsOfEntry = entry.Properties
                    .Where(m => m.Metadata.PropertyInfo != null && m.Metadata.PropertyInfo.PropertyType == typeof(string))
                    .Select(m => m.Metadata.PropertyInfo!.Name);
                foreach (var stringField in stringFieldsOfEntry)
                {
                    var property = entry.Property(stringField);
                    property.CurrentValue = !string.IsNullOrWhiteSpace((string?)property.CurrentValue) ? property.CurrentValue : null;
                }
            }
        }
    }
}
