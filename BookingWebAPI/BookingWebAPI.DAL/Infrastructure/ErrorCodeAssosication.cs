using BookingWebAPI.DAL.Enums;

namespace BookingWebAPI.DAL.Infrastructure
{
    internal class ErrorCodeAssosication
    {
        public ErrorCodeAssosication(string databaseObjectName, SqlServerErrorCode errorCode, string applicationErrorCode)
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
