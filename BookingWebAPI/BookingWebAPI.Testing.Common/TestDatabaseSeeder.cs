using BookingWebAPI.Common.Models;
using BookingWebAPI.DAL;

namespace BookingWebAPI.Testing.Common
{
    public static class TestDatabaseSeeder
    {
        public static readonly string DummyPasswordHash = new string('A', 60);

        public static readonly IEnumerable<Site> Sites = new Site[]
        {
            new Site { Id = new Guid(Constants.ActiveSiteId), Name = "Szeged site", Country = "Hungary", State = null, County = "Csongrád-Csanád", City = "Szeged", Street = "Jokai", HouseOrFlatNumber = "1", ZipCode = "6720", Description = "", IsDeleted = false },
            new Site { Id = new Guid(Constants.DeletedSiteId), Name = "Szeged site - Deleted", Country = "Hungary", State = null, County = "Csongrád-Csanád", City = "Szeged", Street = "Jokai", HouseOrFlatNumber = "2", ZipCode = "6720", Description = "", IsDeleted = true },
        };

        public static readonly IEnumerable<BookingWebAPIUser> Users = new BookingWebAPIUser[]
        {
            new BookingWebAPIUser { Id = new Guid(Constants.ActiveUserId), Email = Constants.ActiveUserEmail, UserName = Constants.ActiveUserUserName, PasswordHash = DummyPasswordHash, EmailConfirmed = true, IsDeleted = false },
            new BookingWebAPIUser { Id = new Guid(Constants.DeletedUserId), Email = Constants.DeletedUserEmail, UserName = Constants.DeletedUserUserName, PasswordHash = DummyPasswordHash, EmailConfirmed = true, IsDeleted = true }
        };

        internal static void SeedTestData(this BookingWebAPIDbContext dbContext)
        {
            dbContext.Sites.AddRange(Sites);
            dbContext.Users.AddRange(Users);
            dbContext.SaveChanges();
        }

        public static class Constants
        {
            public const string EmptyId = "00000000-0000-0000-0000-000000000000";

            // Site
            public const string ActiveSiteId = "b492810f-9cc3-4586-bdfd-e994b94aeb1a";
            public const string DeletedSiteId = "07c3c477-3a23-4c2f-809a-eae8f26a90d5";

            // BookingWebAPIUser
            public const string ActiveUserId = "81b09273-dbe3-4eec-9455-d6aad496b28d";
            public const string DeletedUserId = "2a3b3ca5-c91e-4960-9e1c-42852fb48bf7";
            public const string ActiveUserEmail = "active@emailProvider.com";
            public const string DeletedUserEmail = "deleted@emailProvider.com";
            public const string ActiveUserUserName = "activeUserName";
            public const string DeletedUserUserName = "deletedUserName";
            public const string NotExistingUserEmail = "newUser@emailProvider.com";
            public const string NotExistingUserUserName = "newUserName";
        }
    }
}
