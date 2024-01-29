using BookingWebAPI.Common.Enums;
using BookingWebAPI.Common.Models;

namespace BookingWebAPI.Services.Interfaces
{
    public interface IEmailConfirmationAttemptService
    {
        public Task<EmailConfirmationAttempt> CreateOrUpdateAsync(EmailConfirmationAttempt entity);

        public Task<EmailConfirmationAttempt?> GetAsync(Guid id);

        public Task<IEnumerable<EmailConfirmationAttempt>> GetByStatusAsync(Guid userId, EmailConfirmationStatus status);
    }
}
