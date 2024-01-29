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

        public override IEnumerable<ErrorCodeAssociation> ErrorCodeAssosications => new ErrorCodeAssociation[]
        {
            new ErrorCodeAssociation(DatabaseConstraintNames.Site_StateCountry_CK, SqlServerErrorCode.ConstraintViolated, ApplicationErrorCodes.SiteStateOrCountryNeeded),
            new ErrorCodeAssociation(DatabaseConstraintNames.Site_Name_UQ, SqlServerErrorCode.CannotInsertDuplicate, ApplicationErrorCodes.SiteNameMustBeUnique),
            new ErrorCodeAssociation(nameof(Site.Name), SqlServerErrorCode.CannotInsertNull, ApplicationErrorCodes.SiteNameRequired),
            new ErrorCodeAssociation(nameof(Site.Name), SqlServerErrorCode.StringOrBinaryTruncated, ApplicationErrorCodes.SiteNameTooLong),
            new ErrorCodeAssociation(nameof(Site.Description), SqlServerErrorCode.StringOrBinaryTruncated, ApplicationErrorCodes.SiteDescriptionTooLong),
            new ErrorCodeAssociation(nameof(Site.Country), SqlServerErrorCode.CannotInsertNull, ApplicationErrorCodes.SiteCountryRequired),
            new ErrorCodeAssociation(nameof(Site.Country), SqlServerErrorCode.StringOrBinaryTruncated, ApplicationErrorCodes.SiteCountryTooLong),
            new ErrorCodeAssociation(nameof(Site.ZipCode), SqlServerErrorCode.CannotInsertNull, ApplicationErrorCodes.SiteZipCodeRequired),
            new ErrorCodeAssociation(nameof(Site.ZipCode), SqlServerErrorCode.StringOrBinaryTruncated, ApplicationErrorCodes.SiteZipCodeTooLong),
            new ErrorCodeAssociation(nameof(Site.State), SqlServerErrorCode.StringOrBinaryTruncated, ApplicationErrorCodes.SiteStateTooLong),
            new ErrorCodeAssociation(nameof(Site.County), SqlServerErrorCode.StringOrBinaryTruncated, ApplicationErrorCodes.SiteCountyTooLong),
            new ErrorCodeAssociation(nameof(Site.City), SqlServerErrorCode.CannotInsertNull, ApplicationErrorCodes.SiteCityRequired),
            new ErrorCodeAssociation(nameof(Site.City), SqlServerErrorCode.StringOrBinaryTruncated, ApplicationErrorCodes.SiteCityTooLong),
            new ErrorCodeAssociation(nameof(Site.Street), SqlServerErrorCode.CannotInsertNull, ApplicationErrorCodes.SiteStreetRequired),
            new ErrorCodeAssociation(nameof(Site.Street), SqlServerErrorCode.StringOrBinaryTruncated, ApplicationErrorCodes.SiteStreetTooLong),
            new ErrorCodeAssociation(nameof(Site.HouseOrFlatNumber), SqlServerErrorCode.CannotInsertNull, ApplicationErrorCodes.SiteHouseOrFlatNumberRequired),
            new ErrorCodeAssociation(nameof(Site.HouseOrFlatNumber), SqlServerErrorCode.StringOrBinaryTruncated, ApplicationErrorCodes.SiteHouseOrFlatNumberTooLong)
        };
    }
}
