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
    }
}
