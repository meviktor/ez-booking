namespace BookingWebAPI.Common.ErrorCodes
{
    public class ApplicationErrorCodes
    {
        // General
        public const string UnknownError = nameof(UnknownError);
        public const string EntityNotFound = nameof(EntityNotFound);
        public const string CannotAuthenticate = nameof(CannotAuthenticate);

        // Sites
        public const string SiteDoesNotExist = nameof(SiteDoesNotExist);
        public const string SiteStateOrCountryNeeded = nameof(SiteStateOrCountryNeeded);
        public const string SiteNameMustBeUnique = nameof(SiteNameMustBeUnique);
        public const string SiteNameRequired = nameof(SiteNameRequired);
        public const string SiteNameTooLong = nameof(SiteNameTooLong);
        public const string SiteDescriptionTooLong = nameof(SiteDescriptionTooLong);
        public const string SiteCountryRequired = nameof(SiteCountryRequired);
        public const string SiteCountryTooLong = nameof(SiteCountryTooLong);
        public const string SiteZipCodeRequired = nameof(SiteZipCodeRequired);
        public const string SiteZipCodeTooLong = nameof(SiteZipCodeTooLong);
        public const string SiteStateTooLong = nameof(SiteStateTooLong);
        public const string SiteCountyTooLong = nameof(SiteCountyTooLong);
        public const string SiteCityRequired = nameof(SiteCityRequired);
        public const string SiteCityTooLong = nameof(SiteCityTooLong);
        public const string SiteStreetRequired = nameof(SiteStreetRequired);
        public const string SiteStreetTooLong = nameof(SiteStreetTooLong);
        public const string SiteHouseOrFlatNumberRequired = nameof(SiteHouseOrFlatNumberRequired);
        public const string SiteHouseOrFlatNumberTooLong = nameof(SiteHouseOrFlatNumberTooLong);

        // Resource
        public const string ResourceNameMustBeUnique = nameof(ResourceNameMustBeUnique);

        // ResourceCategory
        public const string ResourceCategoryNameMustBeUnique = nameof(ResourceCategoryNameMustBeUnique);

        // BookingWebAPIUser
        public const string UserDoesNotExist = nameof(UserDoesNotExist);
        public const string UserUserNameMustBeUnique = nameof(UserUserNameMustBeUnique);
        public const string UserEmailMustBeUnique = nameof(UserEmailMustBeUnique);
        public const string UserEmailTooLong = nameof(UserEmailTooLong);
        public const string UserEmailRequired = nameof(UserEmailRequired);
        public const string UserEmailInvalidFormat = nameof(UserEmailInvalidFormat);
        public const string UserUserNameTooLong = nameof(UserUserNameTooLong);
        public const string UserUserNameRequired = nameof(UserUserNameRequired);
        public const string LoginInvalidUserNameOrPassword = nameof(LoginInvalidUserNameOrPassword);
        public const string LoginPasswordRequired = nameof(LoginPasswordRequired);
        public const string LoginEmailNotConfirmed = nameof(LoginEmailNotConfirmed);
        public const string UserLockedOut = nameof(UserLockedOut);
        public const string UserFirstNameRequired = nameof(UserFirstNameRequired);
        public const string UserLastNameRequired = nameof(UserLastNameRequired);
        public const string UserSiteIdRequired = nameof(UserSiteIdRequired);
        public const string UserPasswordNotValidByPolicy = nameof(UserPasswordNotValidByPolicy);

        // BookingWebAPISetting
        public const string SettingWrongSettingType = nameof(SettingWrongSettingType);
        public const string SettingNotKnownSettingType = nameof(SettingNotKnownSettingType);
    }
}
