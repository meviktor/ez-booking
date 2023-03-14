using BookingWebAPI.Common.Constants;
using BookingWebAPI.Common.ErrorCodes;
using BookingWebAPI.Common.Models;
using BookingWebAPI.DAL.Enums;
using BookingWebAPI.DAL.Infrastructure;
using BookingWebAPI.DAL.Interfaces;

namespace BookingWebAPI.DAL.Repositories
{
    internal class UserRepository : CRUDRepository<BookingWebAPIUser>, IUserRepository
    {
        public UserRepository(BookingWebAPIDbContext dbContext) 
            : base(dbContext)
        {}

        protected override IEnumerable<ErrorCodeAssosication> ErrorCodeAssosications => new ErrorCodeAssosication[]
        {
            new ErrorCodeAssosication(DatabaseConstraintNames.User_UserName_UQ, SqlServerErrorCode.CannotInsertDuplicate, ApplicationErrorCodes.UserUserNameMustBeUnique),
            new ErrorCodeAssosication(DatabaseConstraintNames.User_Email_UQ, SqlServerErrorCode.CannotInsertDuplicate, ApplicationErrorCodes.UserEmailMustBeUnique),
            new ErrorCodeAssosication(nameof(BookingWebAPIUser.Email), SqlServerErrorCode.CannotInsertNull, ApplicationErrorCodes.UserEmailRequired),
            new ErrorCodeAssosication(nameof(BookingWebAPIUser.Email), SqlServerErrorCode.StringOrBinaryTruncated, ApplicationErrorCodes.UserEmailTooLong),
            new ErrorCodeAssosication(nameof(BookingWebAPIUser.UserName), SqlServerErrorCode.CannotInsertNull, ApplicationErrorCodes.UserUserNameRequired),
            new ErrorCodeAssosication(nameof(BookingWebAPIUser.UserName), SqlServerErrorCode.StringOrBinaryTruncated, ApplicationErrorCodes.UserUserNameTooLong)
        };
    }
}
