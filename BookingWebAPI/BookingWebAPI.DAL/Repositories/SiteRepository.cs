using BookingWebAPI.Common.Constants;
using BookingWebAPI.Common.ErrorCodes;
using BookingWebAPI.Common.Exceptions;
using BookingWebAPI.Common.Models;
using BookingWebAPI.DAL.Interfaces;
using Microsoft.Data.SqlClient;

namespace BookingWebAPI.DAL.Repositories
{
    internal class SiteRepository : CRUDRepository<Site>, ISiteRepository
    {
        public SiteRepository(BookingWebAPIDbContext dbContext) 
            : base(dbContext)
        {}

        public override async Task<Site> CreateOrUpdateAsync(Site site)
        {
            try
            {
               return await base.CreateOrUpdateAsync(site);
            }
            catch (SqlException e)
            {
                if (e.Message.Contains(DatabaseConstraintNames.Site_StateCountry_CK))
                {
                    throw new DALException(ApplicationErrorCodes.SiteStateOrCountryNeeded);
                }
                else if (e.Message.Contains(DatabaseConstraintNames.Site_Name_UQ))
                {
                    throw new DALException(ApplicationErrorCodes.SiteNameMustBeUnique);
                }
                else throw e;
            }
        }
    }
}
