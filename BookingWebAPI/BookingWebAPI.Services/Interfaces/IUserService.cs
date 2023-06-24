using BookingWebAPI.Common.Models;

namespace BookingWebAPI.Services.Interfaces
{
    public interface IUserService
    {
        Task<(BookingWebAPIUser, string)> Authenticate(string emailAddress, string password);

        Task<BookingWebAPIUser> Register(string emailAddress, Guid siteId, string firstName, string lastName);

        Task<BookingWebAPIUser> ConfirmRegistration(Guid userId, Guid token, string password);

        Task<BookingWebAPIUser> FindUserForEmailConfirmation(Guid token);

        Task<BookingWebAPIUser?> GetAsync(Guid id);
    }
}
