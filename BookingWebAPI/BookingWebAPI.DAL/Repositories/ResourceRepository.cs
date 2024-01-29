using BookingWebAPI.Common.Constants;
using BookingWebAPI.Common.ErrorCodes;
using BookingWebAPI.Common.Exceptions;
using BookingWebAPI.Common.Models;
using BookingWebAPI.DAL.Interfaces;
using Microsoft.Data.SqlClient;

namespace BookingWebAPI.DAL.Repositories
{
    internal class ResourceRepository : CRUDRepository<Resource>, IResourceRepository
    {
        public ResourceRepository(BookingWebAPIDbContext dbContext) 
            : base(dbContext)
        {}

        // TODO: revise this!
        public override async Task<Resource> CreateOrUpdateAsync(Resource resource)
        {
            try
            {
                return await base.CreateOrUpdateAsync(resource);
            }
            catch (SqlException e)
            {
                if (e.Message.Contains(DatabaseConstraintNames.Resource_Name_UQ))
                {
                    throw new DALException(ApplicationErrorCodes.ResourceNameMustBeUnique);
                }
                else throw e;
            }
        }
    }
}
