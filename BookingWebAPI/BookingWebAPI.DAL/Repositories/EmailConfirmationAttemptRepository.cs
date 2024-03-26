using BookingWebAPI.Common.Constants;
using BookingWebAPI.Common.Enums;
using BookingWebAPI.Common.ErrorCodes;
using BookingWebAPI.Common.Models;
using BookingWebAPI.DAL.Enums;
using BookingWebAPI.DAL.Infrastructure;
using BookingWebAPI.DAL.Interfaces;
using BookingWebAPI.DAL.Repositories.Utils;
using Microsoft.EntityFrameworkCore;

namespace BookingWebAPI.DAL.Repositories
{
    internal class EmailConfirmationAttemptRepository : RepositoryBase<EmailConfirmationAttempt>, IEmailConfirmationAttemptRepository
    {
        public EmailConfirmationAttemptRepository(BookingWebAPIDbContext dbContext) : base(dbContext)
        {
        }
        public override IEnumerable<ErrorCodeAssociation> ErrorCodeAssosications => new ErrorCodeAssociation[]
        {
            new ErrorCodeAssociation("FK_EmailConfirmationAttempts_Users_UserId", SqlServerErrorCode.ConstraintViolated, ApplicationErrorCodes.EmailConfirmationUserIdRequired)
        };

        public async Task<EmailConfirmationAttempt> CreateOrUpdateAsync(EmailConfirmationAttempt entity) => await this.CreateOrUpdateInternalAsync(entity);

        public async Task<IEnumerable<EmailConfirmationAttempt>> GetByStatusAsync(Guid userId, EmailConfirmationStatus status) => await DbContext.EmailConfirmationAttempts.Where(eca => eca.UserId == userId && eca.Status == status).ToListAsync();
    }
}
