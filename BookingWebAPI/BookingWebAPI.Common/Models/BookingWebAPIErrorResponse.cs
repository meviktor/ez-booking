using System.Net;

namespace BookingWebAPI.Common.Models
{
    public class BookingWebAPIErrorResponse
    {
        public BookingWebAPIErrorResponse(HttpStatusCode statusCode, string errorCode)
        {
            StatusCode = statusCode;
            ErrorCode = errorCode;
        }

        public HttpStatusCode StatusCode { get; private set; }

        public string ErrorCode { get; private set; }
    }
}
