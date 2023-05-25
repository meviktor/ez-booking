using BookingWebAPI.Common.Models;

namespace BookingWebAPI.DAL.Interfaces
{
    public interface IUserRepository : ICRUDRepository<BookingWebAPIUser>
    {
        Task<BookingWebAPIUser?> FindByUserEmail(string userName);

        Task<BookingWebAPIUser?> FindByEmailVerificationToken(Guid token);

        Task<bool> ExistsByEmailVerificationToken(Guid token);

        Task<bool> ExistsByUserName(string userName);

        Task<bool> ExistsByEmail(string email);
    }
}
