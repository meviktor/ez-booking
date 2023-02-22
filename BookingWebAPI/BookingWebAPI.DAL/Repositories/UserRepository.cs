using BookingWebAPI.Common.Models;
using BookingWebAPI.DAL.Interfaces;

namespace BookingWebAPI.DAL.Repositories
{
    internal class UserRepository : CRUDRepository<BookingWebAPIUser>, IUserRepository
    {
        public UserRepository(BookingWebAPIDbContext dbContext) 
            : base(dbContext)
        {}
    }
}
