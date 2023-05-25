namespace BookingWebAPI.Common.Constants
{
    public class DatabaseConstraintNames
    {
        // Resource
        public const string Resource_Name_UQ = "UQ_Resource_Name";

        // ResourceCategory
        public const string ResourceCategory_Name_UQ = "UQ_ResourceCategory_Name";

        // Site
        public const string Site_Name_UQ = "UQ_Site_Name";
        public const string Site_StateCountry_CK = "CK_Site_StateCounty";

        // BookingWebAPIUser
        public const string User_UserName_UQ = "UQ_User_UserName";
        public const string User_Email_UQ = "UQ_User_Email";
        public const string User_PasswordHash_ColumnType = "CHAR(60)";

        // BookingWebAPISetting
        public const string Setting_NameCategory_UQ = "UQ_Setting_NameCategory";
    }
}
