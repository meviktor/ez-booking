namespace BookingWebAPI.Common.ErrorCodes
{
    public class ApplicationErrorCodes
    {
        // General
        public const string UnknownError = nameof(UnknownError);
        public const string EntityNotFound = nameof(EntityNotFound);

        // Sites
        public const string SiteStateOrCountryNeeded = nameof(SiteStateOrCountryNeeded);
        public const string SiteNameMustBeUnique = nameof(SiteNameMustBeUnique);

        // Resource
        public const string ResourceNameMustBeUnique = nameof(ResourceNameMustBeUnique);

        // ResourceCategory
        public const string ResourceCategoryNameMustBeUnique = nameof(ResourceNameMustBeUnique);

        // BookingWebAPIUser
        public const string UserUserNameMustBeUnique = nameof(UserUserNameMustBeUnique);
        public const string UserEmailMustBeUnique = nameof(UserEmailMustBeUnique);
        public const string UserEmailTooLong = nameof(UserEmailTooLong);
        public const string UserEmailRequired = nameof(UserEmailRequired);
        public const string UserEmailInvalidFormat = nameof(UserEmailInvalidFormat);
        public const string UserUserNameTooLong = nameof(UserUserNameTooLong);
        public const string UserUserNameRequired = nameof(UserUserNameRequired);
    }
}
