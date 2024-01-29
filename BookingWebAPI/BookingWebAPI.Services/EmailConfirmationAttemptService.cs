using BookingWebAPI.Common.Enums;
using BookingWebAPI.Common.Models;
using BookingWebAPI.DAL.Interfaces;
using BookingWebAPI.Services.Interfaces;

namespace BookingWebAPI.Services
{
    internal class EmailConfirmationAttemptService : IEmailConfirmationAttemptService
    {
        private IEmailConfirmationAttemptRepository _emailConfirmationAttemptRepository;

        public EmailConfirmationAttemptService(IEmailConfirmationAttemptRepository emailConfirmationAttemptRepository) =>
            _emailConfirmationAttemptRepository = emailConfirmationAttemptRepository;

        public async Task<EmailConfirmationAttempt> CreateOrUpdateAsync(EmailConfirmationAttempt entity) => await _emailConfirmationAttemptRepository.CreateOrUpdateAsync(entity);

        public async Task<EmailConfirmationAttempt?> GetAsync(Guid id) => await _emailConfirmationAttemptRepository.GetAsync(id);

        public async Task<IEnumerable<EmailConfirmationAttempt>> GetByStatusAsync(Guid userId, EmailConfirmationStatus status) => await _emailConfirmationAttemptRepository.GetByStatusAsync(userId, status);
    }
}
