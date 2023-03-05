using BookingWebAPI.Common.Constants;
using BookingWebAPI.Common.ErrorCodes;
using BookingWebAPI.Common.Exceptions;
using BookingWebAPI.Common.Models;
using BookingWebAPI.DAL.Enums;
using BookingWebAPI.DAL.Interfaces;
using Microsoft.Data.SqlClient;
using Microsoft.EntityFrameworkCore;

namespace BookingWebAPI.DAL.Repositories
{
    internal class UserRepository : CRUDRepository<BookingWebAPIUser>, IUserRepository
    {
        public UserRepository(BookingWebAPIDbContext dbContext) 
            : base(dbContext)
        {}

        public override async Task<BookingWebAPIUser> CreateOrUpdateAsync(BookingWebAPIUser user)
        {
            try
            {
                return await base.CreateOrUpdateAsync(user);
            }
            catch (DbUpdateException e) when (e.InnerException is SqlException sqlEx)
            {
                if (sqlEx.Message.Contains(DatabaseConstraintNames.User_UserName_UQ))
                {
                    throw new DALException(ApplicationErrorCodes.UserUserNameMustBeUnique);
                }
                else if (sqlEx.Message.Contains(DatabaseConstraintNames.User_Email_UQ))
                {
                    throw new DALException(ApplicationErrorCodes.UserEmailMustBeUnique);
                }
                else if (sqlEx.Message.Contains(nameof(BookingWebAPIUser.Email)) && sqlEx.Number == (int)SqlServerErrorCode.CannotInsertNull)
                {
                    throw new DALException(ApplicationErrorCodes.UserEmailRequired);
                }
                else if (sqlEx.Message.Contains(nameof(BookingWebAPIUser.Email)) && sqlEx.Number == (int)SqlServerErrorCode.StringOrBinaryTruncated)
                {
                    throw new DALException(ApplicationErrorCodes.UserEmailTooLong);
                }
                else if (sqlEx.Message.Contains(nameof(BookingWebAPIUser.UserName)) && sqlEx.Number == (int)SqlServerErrorCode.CannotInsertNull)
                {
                    throw new DALException(ApplicationErrorCodes.UserUserNameRequired);
                }
                else if (sqlEx.Message.Contains(nameof(BookingWebAPIUser.UserName)) && sqlEx.Number == (int)SqlServerErrorCode.StringOrBinaryTruncated)
                {
                    throw new DALException(ApplicationErrorCodes.UserUserNameTooLong);
                }
                else throw e;
            }
        }
    }
}
