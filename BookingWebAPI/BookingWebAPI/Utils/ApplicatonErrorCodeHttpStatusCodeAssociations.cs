using BookingWebAPI.Common.ErrorCodes;
using BookingWebAPI.Common.Exceptions;
using System.Net;

namespace BookingWebAPI.Utils
{
    public static class ApplicatonErrorCodeHttpStatusCodeAssociations
    {
        private static List<(string[], HttpStatusCode)> _errorCodesByHttpStatusCode = new List<(string[], HttpStatusCode)>()
        {
            (new string[] { 
                ApplicationErrorCodes.UnknownError,
                ApplicationErrorCodes.SettingWrongSettingType,
                ApplicationErrorCodes.SettingNotKnownSettingType
            }, HttpStatusCode.InternalServerError),
            (new string[] {
                ApplicationErrorCodes.EntityNotFound,
                // Sites
                ApplicationErrorCodes.SiteStateOrCountryNeeded,
                ApplicationErrorCodes.SiteNameMustBeUnique,
                ApplicationErrorCodes.SiteNameRequired,
                ApplicationErrorCodes.SiteNameTooLong,
                ApplicationErrorCodes.SiteDescriptionTooLong,
                ApplicationErrorCodes.SiteCountryRequired,
                ApplicationErrorCodes.SiteCountryTooLong,
                ApplicationErrorCodes.SiteZipCodeRequired,
                ApplicationErrorCodes.SiteZipCodeTooLong,
                ApplicationErrorCodes.SiteStateTooLong,
                ApplicationErrorCodes.SiteCountyTooLong,
                ApplicationErrorCodes.SiteCityRequired,
                ApplicationErrorCodes.SiteCityTooLong,
                ApplicationErrorCodes.SiteStreetRequired,
                ApplicationErrorCodes.SiteStreetTooLong,
                ApplicationErrorCodes.SiteHouseOrFlatNumberRequired,
                ApplicationErrorCodes.SiteHouseOrFlatNumberTooLong,
                ApplicationErrorCodes.SiteDoesNotExist,
                // Resource
                ApplicationErrorCodes.ResourceNameMustBeUnique,
                // ResourceCategory
                ApplicationErrorCodes.ResourceCategoryNameMustBeUnique,
                // BookingWebAPIUser
                ApplicationErrorCodes.UserDoesNotExist,
                ApplicationErrorCodes.UserUserNameMustBeUnique,
                ApplicationErrorCodes.UserEmailMustBeUnique,
                ApplicationErrorCodes.UserEmailTooLong,
                ApplicationErrorCodes.UserEmailRequired,
                ApplicationErrorCodes.UserEmailInvalidFormat,
                ApplicationErrorCodes.UserUserNameTooLong,
                ApplicationErrorCodes.UserUserNameRequired,
                ApplicationErrorCodes.UserSiteIdRequired,
                ApplicationErrorCodes.UserFirstNameRequired,
                ApplicationErrorCodes.UserLastNameRequired,
                ApplicationErrorCodes.UserPasswordNotValidByPolicy,
                ApplicationErrorCodes.LoginPasswordRequired
            }, HttpStatusCode.BadRequest),
            (new string[]{
                ApplicationErrorCodes.UserLockedOut
            }, HttpStatusCode.Forbidden),
            (new string[]{
                ApplicationErrorCodes.LoginInvalidUserNameOrPassword,
                ApplicationErrorCodes.CannotAuthenticate,
                ApplicationErrorCodes.LoginInvalidUserNameOrPassword,
                ApplicationErrorCodes.LoginEmailNotConfirmed,
            }, HttpStatusCode.Unauthorized)
        };

        private static Dictionary<string, HttpStatusCode> _errorCodeStatusCodeMappings;

        static ApplicatonErrorCodeHttpStatusCodeAssociations() => _errorCodeStatusCodeMappings = _errorCodesByHttpStatusCode
            .SelectMany(group => group.Item1.Select(item => new { ErrorCode = item, StatusCode = group.Item2 }))
            .ToDictionary(x => x.ErrorCode, x => x.StatusCode);

        /// <summary>
        /// Returns an appropriate <see cref="HttpStatusCode"/> for the provided application error code.
        /// Throws an <see cref="ArgumentException"/> if the passed error code does not exist or no status code has been set.
        /// </summary>
        /// <param name="applicationErrorCode">The error code of the <see cref="BookingWebAPIException"/> thrown by the application.</param>
        /// <returns></returns>
        /// <exception cref="ArgumentException"></exception>
        public static HttpStatusCode GetHttpStatusCode(string applicationErrorCode)
        {
            var key = _errorCodeStatusCodeMappings.Keys.SingleOrDefault(k => k == applicationErrorCode);
            return key != null
                ? _errorCodeStatusCodeMappings[key]
                : throw new ArgumentException($"Error code '{applicationErrorCode}' does not exist, or no status code has been assigned to it.");
        }
    }
}
