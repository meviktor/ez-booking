namespace BookingWebAPI.Common.Models
{
    public class BookingWebAPIErrorResponse
    {
        public BookingWebAPIErrorResponse(string errorCode) => ErrorCode = errorCode;

        public string ErrorCode { get; private set; }
    }
}
