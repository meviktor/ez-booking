using BookingWebAPI.Common.Constants;
using BookingWebAPI.Common.ErrorCodes;
using BookingWebAPI.Common.Models;
using BookingWebAPI.DAL.Enums;
using BookingWebAPI.DAL.Infrastructure;
using BookingWebAPI.DAL.Interfaces;

namespace BookingWebAPI.DAL.Repositories
{
    internal class SiteRepository : CRUDRepository<Site>, ISiteRepository
    {
        public SiteRepository(BookingWebAPIDbContext dbContext) 
            : base(dbContext)
        {}

        protected override IEnumerable<ErrorCodeAssosication> ErrorCodeAssosications => new ErrorCodeAssosication[]
        {
            new ErrorCodeAssosication(DatabaseConstraintNames.Site_StateCountry_CK, SqlServerErrorCode.CannotInsertDuplicate, ApplicationErrorCodes.SiteStateOrCountryNeeded),
            new ErrorCodeAssosication(DatabaseConstraintNames.Site_Name_UQ, SqlServerErrorCode.CannotInsertDuplicate, ApplicationErrorCodes.SiteNameMustBeUnique),
            new ErrorCodeAssosication(nameof(Site.Name), SqlServerErrorCode.CannotInsertNull, ApplicationErrorCodes.SiteNameRequired),
            new ErrorCodeAssosication(nameof(Site.Name), SqlServerErrorCode.StringOrBinaryTruncated, ApplicationErrorCodes.SiteNameTooLong),
            new ErrorCodeAssosication(nameof(Site.Description), SqlServerErrorCode.StringOrBinaryTruncated, ApplicationErrorCodes.SiteDescriptionTooLong),
            new ErrorCodeAssosication(nameof(Site.Country), SqlServerErrorCode.CannotInsertNull, ApplicationErrorCodes.SiteCountryRequired),
            new ErrorCodeAssosication(nameof(Site.Country), SqlServerErrorCode.StringOrBinaryTruncated, ApplicationErrorCodes.SiteCountryTooLong),
            new ErrorCodeAssosication(nameof(Site.ZipCode), SqlServerErrorCode.CannotInsertNull, ApplicationErrorCodes.SiteZipCodeRequired),
            new ErrorCodeAssosication(nameof(Site.ZipCode), SqlServerErrorCode.StringOrBinaryTruncated, ApplicationErrorCodes.SiteZipCodeTooLong),
            new ErrorCodeAssosication(nameof(Site.State), SqlServerErrorCode.StringOrBinaryTruncated, ApplicationErrorCodes.SiteNameMustBeUnique),
            new ErrorCodeAssosication(nameof(Site.County), SqlServerErrorCode.StringOrBinaryTruncated, ApplicationErrorCodes.SiteNameMustBeUnique),
            new ErrorCodeAssosication(nameof(Site.City), SqlServerErrorCode.CannotInsertNull, ApplicationErrorCodes.SiteCityRequired),
            new ErrorCodeAssosication(nameof(Site.City), SqlServerErrorCode.StringOrBinaryTruncated, ApplicationErrorCodes.SiteCityTooLong),
            new ErrorCodeAssosication(nameof(Site.Street), SqlServerErrorCode.CannotInsertNull, ApplicationErrorCodes.SiteNameMustBeUnique),
            new ErrorCodeAssosication(nameof(Site.Street), SqlServerErrorCode.StringOrBinaryTruncated, ApplicationErrorCodes.SiteNameMustBeUnique),
            new ErrorCodeAssosication(nameof(Site.HouseOrFlatNumber), SqlServerErrorCode.CannotInsertNull, ApplicationErrorCodes.SiteNameMustBeUnique),
            new ErrorCodeAssosication(nameof(Site.HouseOrFlatNumber), SqlServerErrorCode.StringOrBinaryTruncated, ApplicationErrorCodes.SiteNameMustBeUnique)
        };
    }
}
