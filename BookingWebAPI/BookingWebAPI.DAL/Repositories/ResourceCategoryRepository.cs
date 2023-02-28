using BookingWebAPI.Common.Constants;
using BookingWebAPI.Common.ErrorCodes;
using BookingWebAPI.Common.Exceptions;
using BookingWebAPI.Common.Models;
using BookingWebAPI.DAL.Interfaces;
using Microsoft.Data.SqlClient;

namespace BookingWebAPI.DAL.Repositories
{
    internal class ResourceCategoryRepository : CRUDRepository<ResourceCategory>, IResourceCategoryRepository
    {
        public ResourceCategoryRepository(BookingWebAPIDbContext dbContext)
            : base(dbContext)
        {}

        public override async Task<ResourceCategory> CreateOrUpdateAsync(ResourceCategory resourceCategory)
        {
            try
            {
                return await base.CreateOrUpdateAsync(resourceCategory);
            }
            catch (SqlException e)
            {
                if (e.Message.Contains(DatabaseConstraintNames.ResourceCategory_Name_UQ))
                {
                    throw new DALException(ApplicationErrorCodes.SiteStateOrCountryNeeded);
                }
                else throw e;
            }
        }
    }
}
