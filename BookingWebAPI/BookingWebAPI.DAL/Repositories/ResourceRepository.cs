using BookingWebAPI.Common.Models;
using BookingWebAPI.DAL.Interfaces;

namespace BookingWebAPI.DAL.Repositories
{
    internal class ResourceRepository : CRUDRepository<Resource>, IResourceRepository
    {
        public ResourceRepository(BookingWebAPIDbContext dbContext) 
            : base(dbContext)
        {}
    }
}
