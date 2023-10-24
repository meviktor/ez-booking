using BookingWebAPI.Common.Constants;
using BookingWebAPI.Common.Enums;
using BookingWebAPI.Common.Models;
using BookingWebAPI.DAL;

namespace BookingWebAPI.Testing.Common
{
    public static class TestDatabaseSeeder
    {
        public static readonly string DummyPasswordHash = new string('A', 60);

        public static readonly IEnumerable<Site> Sites = new Site[]
        {
            new Site { Id = new Guid(Constants.ActiveSiteId), Name = Constants.ActiveSiteName, Country = "Hungary", State = null, County = "Csongrád-Csanád", City = "Szeged", Street = "Jokai", HouseOrFlatNumber = "1", ZipCode = "6720", Description = "", IsDeleted = false },
            new Site { Id = new Guid(Constants.DeletedSiteId), Name = Constants.DeletedSiteName, Country = "Hungary", State = null, County = "Csongrád-Csanád", City = "Szeged", Street = "Jokai", HouseOrFlatNumber = "2", ZipCode = "6720", Description = "", IsDeleted = true },
        };

        public static readonly IEnumerable<BookingWebAPIUser> Users = new BookingWebAPIUser[]
        {
            new BookingWebAPIUser { Id = new Guid(Constants.ActiveUserId), Email = Constants.ActiveUserEmail, UserName = Constants.ActiveUserUserName, PasswordHash = DummyPasswordHash, EmailConfirmed = true, IsDeleted = false, SiteId = Guid.Parse(Constants.ActiveSiteId), FirstName = "Jane", LastName = "Doe" },
            new BookingWebAPIUser { Id = new Guid(Constants.DeletedUserId), Email = Constants.DeletedUserEmail, UserName = Constants.DeletedUserUserName, PasswordHash = DummyPasswordHash, EmailConfirmed = true, IsDeleted = true, SiteId = Guid.Parse(Constants.DeletedSiteId), FirstName = "John", LastName = "Doe" }
        };

        public static readonly IEnumerable<BookingWebAPISetting> Settings = new BookingWebAPISetting[]
        {
            new BookingWebAPISetting { Id = new Guid(Constants.ActiveSettingId), Name = Constants.ActiveSettingName, ValueType = SettingValueType.String, RawValue = "rawValueActive", Category = Constants.SettingCategoryTesting, IsDeleted = false },
            // Deleted setting from the same category as the active one
            new BookingWebAPISetting { Id = Guid.NewGuid(), Name = $"{Constants.DeletedSettingName}_{Constants.SettingCategoryTesting}", ValueType = SettingValueType.String, RawValue = "rawValueDeleted", Category = Constants.SettingCategoryTesting, IsDeleted = true },
            // Deleted setting from different category as the active one
            new BookingWebAPISetting { Id = Guid.NewGuid(), Name = $"{Constants.DeletedSettingName}_{SettingCategory.PasswordPolicy}", ValueType = SettingValueType.String, RawValue = "rawValueDeleted", Category = SettingCategory.PasswordPolicy, IsDeleted = true },
            new BookingWebAPISetting { Id = Guid.NewGuid(), Name = ApplicationConstants.LoginMaxAttempts, ValueType = SettingValueType.Integer, RawValue = "5", Category = SettingCategory.Login }
        };

        internal static void SeedTestData(this BookingWebAPIDbContext dbContext)
        {
            dbContext.Sites.AddRange(Sites);
            dbContext.Users.AddRange(Users);
            dbContext.Settings.AddRange(Settings);
            dbContext.SaveChanges();
        }

        public static class Constants
        {
            public const string EmptyId = "00000000-0000-0000-0000-000000000000";

            // Site
            public const string ActiveSiteId = "b492810f-9cc3-4586-bdfd-e994b94aeb1a";
            public const string ActiveSiteName = "Szeged site";
            public const string DeletedSiteId = "07c3c477-3a23-4c2f-809a-eae8f26a90d5";
            public const string DeletedSiteName = "Szeged site - Deleted";
            public const string NotExistingSiteId = "75c98705-45e2-41ec-8a1c-6c5f46f3c10d";
            public const string NotExistingSiteName = "New site experimenting";

            // BookingWebAPIUser
            public const string ActiveUserId = "81b09273-dbe3-4eec-9455-d6aad496b28d";
            public const string DeletedUserId = "2a3b3ca5-c91e-4960-9e1c-42852fb48bf7";
            public const string ActiveUserEmail = "active@emailProvider.com";
            public const string DeletedUserEmail = "deleted@emailProvider.com";
            public const string ActiveUserUserName = "activeUserName";
            public const string DeletedUserUserName = "deletedUserName";
            public const string NotRegisteredUserEmail = "newUser@emailProvider.com";
            public const string NotRegisteredUserUserName = "newUserName";

            // BookingWebAPISetting
            public const string ActiveSettingId = "3b670940-1ffa-47da-ae6e-91416b8e2372";
            public const string ActiveSettingName = "testSettingActive";
            public const string DeletedSettingName = "testSettingDeleted";
            public const SettingCategory SettingCategoryTesting = SettingCategory.Email;
        }
    }
}
