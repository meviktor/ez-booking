using BookingWebAPI.Common.Models;

namespace BookingWebAPI.Services.Interfaces
{
    public interface IUserService
    {
        Task<(BookingWebAPIUser, string)> AuthenticateAsync(string emailAddress, string password);

        Task<BookingWebAPIUser> RegisterAsync(string emailAddress, Guid siteId, string firstName, string lastName);

        Task<BookingWebAPIUser> ConfirmRegistrationAsync(Guid userId, Guid token, string password);

        Task<BookingWebAPIUser> FindUserForEmailConfirmationAsync(Guid token);

        Task<BookingWebAPIUser?> GetAsync(Guid id);
    }
}
