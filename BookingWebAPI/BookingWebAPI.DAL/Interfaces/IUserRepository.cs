using BookingWebAPI.Common.Models;

namespace BookingWebAPI.DAL.Interfaces
{
    public interface IUserRepository : ICRUDRepository<BookingWebAPIUser>
    {
        Task<BookingWebAPIUser?> FindByUserEmailAsync(string userName);

        Task<BookingWebAPIUser?> FindByEmailVerificationTokenAsync(Guid token);

        Task<bool> ExistsByEmailVerificationTokenAsync(Guid token);

        Task<bool> ExistsByUserNameAsync(string userName);

        Task<bool> ExistsByEmailAsync(string email);
    }
}
