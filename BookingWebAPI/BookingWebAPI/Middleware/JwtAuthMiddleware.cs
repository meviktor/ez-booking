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

            var token = context.Request.Headers["Authorization"].FirstOrDefault()?.Split(" ").Last();

            if (token != null)
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
                catch(Exception e)
                {
                    throw new BookingWebAPIException(ApplicationErrorCodes.CannotAuthenticate, "An exception occurred during JWT token validation.", e);
                }
            }

            await _next(context);
        }
    }
}
