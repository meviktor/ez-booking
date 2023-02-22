using BookingWebAPI.Common.Models;
using BookingWebAPI.DAL.Interfaces;

namespace BookingWebAPI.DAL.Repositories
{
    internal class ResourceCategoryRepository : CRUDRepository<ResourceCategory>, IResourceCategoryRepository
    {
        public ResourceCategoryRepository(BookingWebAPIDbContext dbContext)
            : base(dbContext)
        {}
    }
}
