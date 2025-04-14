
using Microsoft.AspNetCore.Authorization;
using Microsoft.OpenApi.Models;
using Swashbuckle.AspNetCore.SwaggerGen;

namespace BookingWebAPI.Utils
{
    public class AllowAnonymousOperationFilter : IOperationFilter
    {
        public void Apply(OpenApiOperation apiOperation, OperationFilterContext context)
        {
            var isAnonymous = context.ApiDescription.CustomAttributes().OfType<AllowAnonymousAttribute>().Any();
            if (!isAnonymous)
            {
                var bearerSecurityScheme = new OpenApiSecurityScheme { Reference = new OpenApiReference { Type = ReferenceType.SecurityScheme, Id = "Bearer" } };
                var bearerTokenReq = new OpenApiSecurityRequirement() { { bearerSecurityScheme, Array.Empty<string>() } };
                apiOperation.Security = new List<OpenApiSecurityRequirement>() { bearerTokenReq };
            }
        }
    }

    public static class SwaggerSecurityDefinition
    {
        public static void Add(SwaggerGenOptions options)
        {
            options.AddSecurityDefinition("Bearer", new OpenApiSecurityScheme
            {
                Type = SecuritySchemeType.Http,
                Scheme = "bearer",
                BearerFormat = "JWT",
                In = ParameterLocation.Header,
                Name = "Authorization",
                Description = "Paste JWT token returned by /api/Users/Authenticate endpoint."
            });

            options.OperationFilter<AllowAnonymousOperationFilter>();
        }
    }
}
