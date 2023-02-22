using BookingWebAPI.Common.Models;
using BookingWebAPI.DAL;

namespace BookingWebAPI.Testing.Common
{
    public static class TestDatabaseSeeder
    {
        public const string EmptyId = "00000000-0000-0000-0000-000000000000";

        public static readonly IEnumerable<Site> Sites = new Site[]
        {
            new Site { Id = new Guid(SiteConstants.ActiveSiteId), Name = "Szeged site", Country = "Hungary", State = null, County = "Csongrád-Csanád", City = "Szeged", Street = "Jokai", HouseOrFlatNumber = "1", ZipCode = "6720", Description = "", IsDeleted = false },
            new Site { Id = new Guid(SiteConstants.DeletedSiteId), Name = "Szeged site - Deleted", Country = "Hungary", State = null, County = "Csongrád-Csanád", City = "Szeged", Street = "Jokai", HouseOrFlatNumber = "2", ZipCode = "6720", Description = "", IsDeleted = true },
        };

        internal static void SeedTestData(this BookingWebAPIDbContext dbContext)
        {
            dbContext.Sites.AddRange(Sites);
            dbContext.SaveChanges();
        }

        public static class SiteConstants
        {
            public const string ActiveSiteId = "b492810f-9cc3-4586-bdfd-e994b94aeb1a";
            public const string DeletedSiteId = "07c3c477-3a23-4c2f-809a-eae8f26a90d5";
        }
    }
}
