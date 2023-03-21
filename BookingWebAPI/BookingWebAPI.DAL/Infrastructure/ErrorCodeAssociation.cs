using BookingWebAPI.DAL.Enums;

namespace BookingWebAPI.DAL.Infrastructure
{
    internal class ErrorCodeAssociation
    {
        public ErrorCodeAssociation(string databaseObjectName, SqlServerErrorCode errorCode, string applicationErrorCode)
        {
            DatabaseObject = databaseObjectName;
            ErrorCode = errorCode;
            ApplicationErrorCode = applicationErrorCode;
        }

        public string DatabaseObject { get; private set; }

        public SqlServerErrorCode ErrorCode { get; private set; }

        public string ApplicationErrorCode { get; private set; }
    }
}
