using BookingWebAPI.Attributes;
using BookingWebAPI.Common.Constants;
using BookingWebAPI.Testing.Common;
using FluentAssertions;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Routing;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Abstractions;
using Microsoft.AspNetCore.Mvc.Controllers;
using Microsoft.AspNetCore.Mvc.Filters;
using Moq;
using NUnit.Framework;
using System.Reflection;
using System.Security.Claims;

namespace BookingWebAPI.Test.Unit
{
    public class AuthorizedEndpointAttributeTests : UnitTestBase
    {
        private static readonly string[] _userClaimTypes = new[] { ApplicationConstants.JwtClaimId, ApplicationConstants.JwtClaimEmail, ApplicationConstants.JwtClaimUserName };

        private Mock<HttpContext> _httpContextMock;
        private AuthorizationFilterContext _authFilterContext;

        [SetUp]
        public void SetUp()
        {
            _httpContextMock = new Mock<HttpContext>();
            _authFilterContext = new AuthorizationFilterContext(new ActionContext(_httpContextMock.Object, new Mock<RouteData>().Object, new Mock<ActionDescriptor>().Object), new List<IFilterMetadata>() { });
        }

        [TestCase(true, true)]
        [TestCase(true, false, ApplicationConstants.JwtClaimId)]
        [TestCase(true, false, ApplicationConstants.JwtClaimEmail)]
        [TestCase(true, false, ApplicationConstants.JwtClaimUserName)]
        public void OnAuthorization_Test(bool authorizationNeeded, bool userAuthenticated, string? missingClaimType = null)
        {
            // prepare
            var customAttributes = authorizationNeeded ? Array.Empty<object>() : new[] { new AllowAnonymousAttribute() };
            var methodInfoMock = new Mock<MethodInfo>();
            methodInfoMock.Setup(mi => mi.GetCustomAttributes(true)).Returns(customAttributes);

            _authFilterContext.ActionDescriptor = new ControllerActionDescriptor { MethodInfo = methodInfoMock.Object };

            _httpContextMock.Setup(http => http.User).Returns(BuildUserClaimsIdentity(_userClaimTypes.Where(ct => ct != missingClaimType)));
            _authFilterContext.HttpContext = _httpContextMock.Object;

            // action
            var authAttr = new AuthorizedAttribute();
            authAttr.OnAuthorization(_authFilterContext);

            // assert
            if (authorizationNeeded && !userAuthenticated && _authFilterContext.Result is JsonResult)
            {
                (_authFilterContext.Result as JsonResult)!.StatusCode.Should().Be(StatusCodes.Status401Unauthorized);
            }
            else if (_authFilterContext.Result is JsonResult && _authFilterContext.Result != null)
            {
                (_authFilterContext.Result as JsonResult)!.StatusCode.Should().NotBe(StatusCodes.Status401Unauthorized);
            }
            else if (_authFilterContext.Result is not JsonResult && _authFilterContext.Result != null)
            {
                Assert.Fail($"Property {nameof(_authFilterContext.Result)} of {nameof(_authFilterContext)} does not have the type of: {nameof(JsonResult)}.");
            }

            // If nothing is set in the AuthorizedAttribute as Result (it sets only 401 status code in current implementation), it means the user is authenticated.
        }

        private static ClaimsPrincipal BuildUserClaimsIdentity(IEnumerable<string> claimTypes) =>
             new ClaimsPrincipal(new ClaimsIdentity[] { new ClaimsIdentity(claimTypes.Select(claimType => new Claim(claimType, string.Empty))) });
    }
}
