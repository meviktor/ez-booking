namespace BookingWebAPI.Common.Exceptions
{
    public class BookingWebAPIException : Exception
    {
        public string ErrorCode { get; private set; }

        public BookingWebAPIException(string errorCode) : this(errorCode, null) { }

        public BookingWebAPIException(string errorCode, string? message) : base(message) => ErrorCode = errorCode;
    }
}
