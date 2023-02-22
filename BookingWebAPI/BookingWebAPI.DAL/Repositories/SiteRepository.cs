using BookingWebAPI.Common.Models;
using BookingWebAPI.DAL.Interfaces;

namespace BookingWebAPI.DAL.Repositories
{
    internal class SiteRepository : CRUDRepository<Site>, ISiteRepository
    {
        public SiteRepository(BookingWebAPIDbContext dbContext) 
            : base(dbContext)
        {}
    }
}
