using BookingWebAPI.Common.Constants;
using BookingWebAPI.Common.ErrorCodes;
using BookingWebAPI.Common.Exceptions;
using Microsoft.IdentityModel.Tokens;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;

namespace BookingWebAPI.Middleware
{
    public class JwtAuthMiddleware
    {
        private readonly RequestDelegate _next;

        public JwtAuthMiddleware(RequestDelegate next) => _next = next;

        public async Task Invoke(HttpContext context, IConfiguration configuration)
        {
            var jwtSecret = configuration["JwtConfig:Secret"];
            if (jwtSecret == null)
            {
                throw new BookingWebAPIException(ApplicationErrorCodes.CannotAuthenticate, "No JWT secret found for authentication.");
            }

            if (TryGetToken(context, out string? token))
            {
                try
                {
                    var tokenHandler = new JwtSecurityTokenHandler();
                    var key = Encoding.ASCII.GetBytes(jwtSecret);
                    tokenHandler.ValidateToken(token, new TokenValidationParameters
                    {
                        ValidateIssuerSigningKey = true,
                        IssuerSigningKey = new SymmetricSecurityKey(key),
                        ValidateIssuer = false,
                        ValidateAudience = false,
                        ClockSkew = TimeSpan.Zero
                    }, out SecurityToken validatedToken);

                    var jwtToken = (JwtSecurityToken)validatedToken;
                    context.User.AddIdentity(new ClaimsIdentity(jwtToken.Claims));
                }
                catch(SecurityTokenExpiredException)
                {
                    context.Response.Cookies.Delete(ApplicationConstants.JwtToken, new CookieOptions { Domain = "ezbooking.com", Secure = true, HttpOnly = true, SameSite = SameSiteMode.None, Path = "/" });
                }
                catch(Exception e)
                {
                    throw new BookingWebAPIException(ApplicationErrorCodes.CannotAuthenticate, "An exception occurred during JWT token validation.", e);
                }
            }

            await _next(context);
        }

        /// <summary>
        /// Attempts to fetch the JWT token from the Authorization header and the request cookies.
        /// </summary>
        /// <param name="context">The <see cref="HttpContext"/> investigated for JWT token.</param>
        /// <param name="token">The JWT token itself. Null if there was no JWT token provided in the request.</param>
        /// <returns>True if the token has been found in any of the defined sources - false otherwise.</returns>
        private static bool TryGetToken(HttpContext context, out string? token)
        {
            token = context.Request.Headers[ApplicationConstants.Authorization].FirstOrDefault()?.Split(" ").Last();
            if (string.IsNullOrWhiteSpace(token))
            {
                token = context.Request.Cookies[ApplicationConstants.JwtToken];
            }
            return !string.IsNullOrWhiteSpace(token);
        }
    }
}
