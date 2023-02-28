using BookingWebAPI.Common.Constants;
using BookingWebAPI.Common.ErrorCodes;
using BookingWebAPI.Common.Exceptions;
using BookingWebAPI.Common.Models;
using BookingWebAPI.DAL.Interfaces;
using Microsoft.Data.SqlClient;

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
            catch (SqlException e)
            {
                if (e.Message.Contains(DatabaseConstraintNames.User_UserName_UQ))
                {
                    throw new DALException(ApplicationErrorCodes.UserUserNameMustBeUnique);
                }
                else if (e.Message.Contains(DatabaseConstraintNames.User_Email_UQ))
                {
                    throw new DALException(ApplicationErrorCodes.UserEmailMustBeUnique);
                }
                else throw e;
            }
        }
    }
}
