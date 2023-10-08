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
        private readonly Mock<IConfiguration> _jwtConfigurationMock;
        private readonly Guid _userId;
        private readonly string _userEmail;
        private readonly string _userName;
        private readonly string _jwtSecret;
        private readonly int _expirationTimeInSecs;

        public JwtAuthMiddlewareTests() : base()
        {
            _jwtConfigurationMock = new Mock<IConfiguration>();
            _userId = Guid.NewGuid();
            _userEmail = "jwtMiddlewareTest@mailaddress.com";
            _userName = "jwtMiddlewareTest";
            _jwtSecret = "testJwtSecret!ForJwtAuthMiddlewareTests";
            _expirationTimeInSecs = 2;
        }

        [SetUp]
        public void SetUp()
        {
            _jwtConfigurationMock.Setup(configuration => configuration["JwtConfig:Secret"]).Returns(_jwtSecret);
            _jwtConfigurationMock.Setup(configuration => configuration["JwtConfig:ValidInSeconds"]).Returns($"{_expirationTimeInSecs}");
        }

        [TestCase(true, false)]
        // If there is no (token in the) Authorization header, no exception will be thrown. In AuthorizedEndpointAttribute the status code will be set to HTTP 401 (Unauthorized).
        [TestCase(false, false)]
        [TestCase(true, true, true, ApplicationErrorCodes.CannotAuthenticate, "An exception occurred during JWT token validation.")]
        public async Task Invoke_Test(bool authHeader, bool tokenExpired, bool exceptionExpected = false, string? errorCodeExpected = null, string? messageExpected = null)
        {
            // prepare
            var jwtAuthMiddleware = new JwtAuthMiddleware((httpContext) => Task.CompletedTask);
            var httpContext = CreateHttpContext(authHeader);

            if(tokenExpired)
            {
                var waitTimeInMs = (_expirationTimeInSecs + 1) * 1000;
                // wait until the token expires
                await Task.Delay(waitTimeInMs);
            }
            var action = async () => await jwtAuthMiddleware.Invoke(httpContext, _jwtConfigurationMock.Object);

            // action & assert
            if(exceptionExpected)
            {
                await action.Should().ThrowExactlyAsync<BookingWebAPIException>().Where(e => e.ErrorCode == errorCodeExpected && e.Message == messageExpected);
            }
            else await action.Should().NotThrowAsync();
        }

        private HttpContext CreateHttpContext(bool authHeader)
        {
            var httpContextMock = new Mock<HttpContext>();
            var requestMock = new Mock<HttpRequest>();
            var headersMock = new Mock<IHeaderDictionary>();

            if (authHeader)
            {
                var authheaders = new StringValues(new string[] { $"Authorization { Utilities.CreateJwtToken(_userId, _userEmail, _userName, _expirationTimeInSecs, _jwtSecret)}" });
                headersMock.Setup(headers => headers["Authorization"]).Returns(authheaders);
                headersMock.Setup(headers => headers.Authorization).Returns(authheaders);
            }
            requestMock.Setup(req => req.Headers).Returns(headersMock.Object);
            httpContextMock.Setup(context => context.Request).Returns(requestMock.Object);
            httpContextMock.Setup(context => context.User).Returns(new ClaimsPrincipal());

            return httpContextMock.Object;
        }
    }
}
