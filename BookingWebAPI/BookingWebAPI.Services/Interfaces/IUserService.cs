using BookingWebAPI.Common.Models;

namespace BookingWebAPI.Services.Interfaces
{
    public interface IUserService
    {
        Task<(BookingWebAPIUser, string)> AuthenticateAsync(string emailAddress, string password);

        Task<BookingWebAPIUser> RegisterAsync(string emailAddress, Guid siteId, string firstName, string lastName);

        Task<Guid> ConfirmEmailRegistrationAsync(Guid attemptId);

        Task<BookingWebAPIUser?> GetAsync(Guid id);
    }
}
