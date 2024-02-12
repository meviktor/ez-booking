using BookingWebAPI.Common.ErrorCodes;
using BookingWebAPI.Common.Exceptions;
using BookingWebAPI.Common.Models;
using BookingWebAPI.Utils;
using Microsoft.AspNetCore.Diagnostics;

namespace BookingWebAPI.Middleware
{
    public class BookingWebAPIExceptionHandler
    {
        public BookingWebAPIExceptionHandler(RequestDelegate next) => _ = next;
      
        public async Task InvokeAsync(HttpContext context)
        {
            var occurredException = context.Features.Get<IExceptionHandlerPathFeature>()?.Error;
            var errorCode = (occurredException as BookingWebAPIException)?.ErrorCode ?? ApplicationErrorCodes.UnknownError;
            var statusCode = ApplicatonErrorCodeHttpStatusCodeAssociations.GetHttpStatusCode(errorCode);
                
            context.Response.StatusCode = (int)statusCode;
            context.Response.ContentType = "application/json";
            await context.Response.WriteAsJsonAsync(new BookingWebAPIErrorResponse(statusCode, errorCode));
        }
    }
}
