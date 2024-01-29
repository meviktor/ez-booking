using BookingWebAPI.Common.Models;

namespace BookingWebAPI.DAL.Interfaces
{
    public interface IUserRepository : ICRUDRepository<BookingWebAPIUser>
    {
        Task<BookingWebAPIUser?> FindByUserEmailAsync(string userName);

        Task<bool> ExistsByUserNameAsync(string userName);

        Task<bool> ExistsByEmailAsync(string email);
    }
}
