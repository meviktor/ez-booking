using BookingWebAPI.Common.Enums;
using BookingWebAPI.Common.Models;

namespace BookingWebAPI.DAL.Interfaces
{
    public interface IEmailConfirmationAttemptRepository : ICRURepository<EmailConfirmationAttempt>
    {
        public Task<IEnumerable<EmailConfirmationAttempt>> GetByStatusAsync(Guid userId, EmailConfirmationStatus status);
    }
}
