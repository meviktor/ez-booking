using BookingWebAPI.Common.Constants;
using BookingWebAPI.Common.ErrorCodes;
using BookingWebAPI.Common.Exceptions;
using BookingWebAPI.Common.Utils;
using BookingWebAPI.Middleware;
using BookingWebAPI.Testing.Common;
using FluentAssertions;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Primitives;
using Moq;
using NUnit.Framework;
using System.Security.Claims;

namespace BookingWebAPI.Test.Unit
{
    internal class JwtAuthMiddlewareTests : UnitTestBase
    {
        private const string _userEmail = "jwtMiddlewareTest@mailaddress.com";
        private const string _userName = "jwtMiddlewareTest";
        private const string _jwtSecret = "testJwtSecret!ForJwtAuthMiddlewareTests";
        private const int _expirationTimeInSecs = 2;

        private readonly Mock<IConfiguration> _jwtConfigurationMock;
        private readonly Guid _userId;

        public JwtAuthMiddlewareTests() : base()
        {
            _jwtConfigurationMock = new Mock<IConfiguration>();
            _userId = Guid.NewGuid();
        }

        [SetUp]
        public void SetUp()
        {
            _jwtConfigurationMock.Setup(configuration => configuration["JwtConfig:Secret"]).Returns(_jwtSecret);
            _jwtConfigurationMock.Setup(configuration => configuration["JwtConfig:ValidInSeconds"]).Returns($"{_expirationTimeInSecs}");
        }

        [TestCase(true, false)]
        [TestCase(false, true)]
        [TestCase(true, false, null, true, ApplicationErrorCodes.CannotAuthenticate, "No JWT secret found for authentication.")]
        [TestCase(false, true, null, true, ApplicationErrorCodes.CannotAuthenticate, "No JWT secret found for authentication.")]
        // If there is no (token in the) Authorization header, no exception will be thrown. In AuthorizedEndpointAttribute the status code will be set to HTTP 401 (Unauthorized).
        [TestCase(false, false)]
        public async Task Invoke_Test(bool authHeader, bool cookie, string jwtSecret = _jwtSecret, bool exceptionExpected = false, string? errorCodeExpected = null, string? messageExpected = null)
        {
            // prepare
            var jwtAuthMiddleware = new JwtAuthMiddleware((httpContext) => Task.CompletedTask);
            var httpContext = CreateHttpContext(authHeader, cookie);

            _jwtConfigurationMock.Setup(configuration => configuration["JwtConfig:Secret"]).Returns(jwtSecret); // "default" JWT secret set up in SetUp() may be overridden with value coming from TestCaseAttribute
            var action = async () => await jwtAuthMiddleware.Invoke(httpContext, _jwtConfigurationMock.Object);

            // action & assert
            if(exceptionExpected)
            {
                await action.Should().ThrowExactlyAsync<BookingWebAPIException>().Where(e => e.ErrorCode == errorCodeExpected && e.Message == messageExpected);
            }
            else await action.Should().NotThrowAsync();
        }

        [Test]
        public async Task Invoke_Test_DeleteExpiredAuthCookie()
        {
            // prepare
            var jwtAuthMiddleware = new JwtAuthMiddleware((httpContext) => Task.CompletedTask);
            var httpContext = CreateHttpContextForDeleteAuthCookie(out Mock<IResponseCookies> responseCookiesMock);

            var waitTimeInMs = (_expirationTimeInSecs + 1) * 1000;
            await Task.Delay(waitTimeInMs); // wait to become expired

            // action
            await jwtAuthMiddleware.Invoke(httpContext, _jwtConfigurationMock.Object);

            // assert
            responseCookiesMock.Verify(r => r.Delete(ApplicationConstants.JwtToken, It.IsAny<CookieOptions>()));
        }

        private HttpContext CreateHttpContext(bool authHeader, bool cookie)
        {
            var httpContextMock = new Mock<HttpContext>();
            var requestMock = new Mock<HttpRequest>();
            var headersMock = new Mock<IHeaderDictionary>();
            var cookiesMock = new Mock<IRequestCookieCollection>();

            if (authHeader)
            {
                var authheaders = new StringValues(new string[] { $"Authorization { Utilities.CreateJwtToken(_userId, _userEmail, _userName, _expirationTimeInSecs, _jwtSecret)}" });
                headersMock.Setup(headers => headers["Authorization"]).Returns(authheaders);
                headersMock.Setup(headers => headers.Authorization).Returns(authheaders);
            }
            if (cookie)
            {
                var jwtToken = Utilities.CreateJwtToken(_userId, _userEmail, _userName, _expirationTimeInSecs, _jwtSecret);
                cookiesMock.Setup(cookies => cookies[ApplicationConstants.JwtToken]).Returns(jwtToken);
            }

            requestMock.Setup(req => req.Headers).Returns(headersMock.Object);
            requestMock.Setup(req => req.Cookies).Returns(cookiesMock.Object);
            httpContextMock.Setup(context => context.Request).Returns(requestMock.Object);
            httpContextMock.Setup(context => context.User).Returns(new ClaimsPrincipal());

            return httpContextMock.Object;
        }

        private HttpContext CreateHttpContextForDeleteAuthCookie(out Mock<IResponseCookies> responseCookiesMock)
        {
            var httpContextMock = new Mock<HttpContext>();
            var requestMock = new Mock<HttpRequest>();
            var responseMock = new Mock<HttpResponse>();
            var headersMock = new Mock<IHeaderDictionary>();
            var cookiesMock = new Mock<IRequestCookieCollection>();
            responseCookiesMock = new Mock<IResponseCookies>();

            var jwtToken = Utilities.CreateJwtToken(_userId, _userEmail, _userName, _expirationTimeInSecs, _jwtSecret);
            cookiesMock.Setup(cookies => cookies[ApplicationConstants.JwtToken]).Returns(jwtToken);

            requestMock.Setup(req => req.Headers).Returns(headersMock.Object);
            requestMock.Setup(req => req.Cookies).Returns(cookiesMock.Object);
            responseMock.Setup(res => res.Cookies).Returns(responseCookiesMock.Object);
            httpContextMock.Setup(context => context.Request).Returns(requestMock.Object);
            httpContextMock.Setup(context => context.Response).Returns(responseMock.Object);

            return httpContextMock.Object;
        }
    }
}
