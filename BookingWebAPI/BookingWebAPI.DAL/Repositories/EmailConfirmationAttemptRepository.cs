using BookingWebAPI.Common.Enums;
using BookingWebAPI.Common.ErrorCodes;
using BookingWebAPI.Common.Interfaces;
using BookingWebAPI.Common.Models;
using BookingWebAPI.DAL.Enums;
using BookingWebAPI.DAL.Infrastructure;
using BookingWebAPI.DAL.Interfaces;
using Microsoft.EntityFrameworkCore;

namespace BookingWebAPI.DAL.Repositories
{
    internal class EmailConfirmationAttemptRepository : CRURepository<EmailConfirmationAttempt>, IEmailConfirmationAttemptRepository
    {
        public EmailConfirmationAttemptRepository(BookingWebAPIDbContext dbContext) : base(dbContext)
        {
        }
        public override IEnumerable<ErrorCodeAssociation> ErrorCodeAssosications => new ErrorCodeAssociation[]
        {
            new ErrorCodeAssociation("FK_EmailConfirmationAttempts_Users_UserId", SqlServerErrorCode.ConstraintViolated, ApplicationErrorCodes.EmailConfirmationUserIdRequired)
        };

        public async Task<IEnumerable<EmailConfirmationAttempt>> GetByStatusAsync(Guid userId, EmailConfirmationStatus status) 
            => await DbContext.EmailConfirmationAttempts.Where(eca => eca.UserId == userId && eca.Status == status).ToListAsync();

        public override async Task<EmailConfirmationAttempt?> GetAsync(Guid id) => await Set.SingleOrDefaultAsync(e => e.Id == id);

        public override async Task<bool> ExistsAsync(Guid id) => await Set.AnyAsync(e => e.Id == id);

        public override IQueryable<EmailConfirmationAttempt> GetAll() => Set;
    }
}
