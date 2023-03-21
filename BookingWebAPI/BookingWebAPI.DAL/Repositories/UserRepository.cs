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

        protected override IEnumerable<ErrorCodeAssociation> ErrorCodeAssosications => new ErrorCodeAssociation[]
        {
            new ErrorCodeAssociation(DatabaseConstraintNames.User_UserName_UQ, SqlServerErrorCode.CannotInsertDuplicate, ApplicationErrorCodes.UserUserNameMustBeUnique),
            new ErrorCodeAssociation(DatabaseConstraintNames.User_Email_UQ, SqlServerErrorCode.CannotInsertDuplicate, ApplicationErrorCodes.UserEmailMustBeUnique),
            new ErrorCodeAssociation(nameof(BookingWebAPIUser.Email), SqlServerErrorCode.CannotInsertNull, ApplicationErrorCodes.UserEmailRequired),
            new ErrorCodeAssociation(nameof(BookingWebAPIUser.Email), SqlServerErrorCode.StringOrBinaryTruncated, ApplicationErrorCodes.UserEmailTooLong),
            new ErrorCodeAssociation(nameof(BookingWebAPIUser.UserName), SqlServerErrorCode.CannotInsertNull, ApplicationErrorCodes.UserUserNameRequired),
            new ErrorCodeAssociation(nameof(BookingWebAPIUser.UserName), SqlServerErrorCode.StringOrBinaryTruncated, ApplicationErrorCodes.UserUserNameTooLong)
        };
    }
}
