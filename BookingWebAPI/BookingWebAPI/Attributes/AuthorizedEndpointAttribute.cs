using BookingWebAPI.Common.Constants;
using BookingWebAPI.Utils;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Controllers;
using Microsoft.AspNetCore.Mvc.Filters;

namespace BookingWebAPI.Attributes
{
    [AttributeUsage(AttributeTargets.Class | AttributeTargets.Method)]
    public class AuthorizedAttribute : Attribute, IAuthorizationFilter
    {
        public void OnAuthorization(AuthorizationFilterContext context)
        {
            var user = context.HttpContext.User;
            if (!HasAllowAnonymousAttribute(context) &&
                !user.HasClaims(ApplicationConstants.JwtClaimId, ApplicationConstants.JwtClaimEmail, ApplicationConstants.JwtClaimUserName))
            {
                // not logged in
                context.Result = new JsonResult(new { message = "Unauthorized" }) { StatusCode = StatusCodes.Status401Unauthorized };
            }
        }

        private static bool HasAllowAnonymousAttribute(AuthorizationFilterContext context)
        {
            var actionDescriptor = context.ActionDescriptor as ControllerActionDescriptor;
            if(actionDescriptor == null) 
            {
                return false;
            }

            var actionAttributes = actionDescriptor.MethodInfo.GetCustomAttributes(inherit: true);
            return actionAttributes.OfType<AllowAnonymousAttribute>().Any();
        }
    }
}
