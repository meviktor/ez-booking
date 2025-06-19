//using Azure.Identity;
using BookingWebAPI.Common.Models.Config;
using BookingWebAPI.DAL;
using BookingWebAPI.Infrastructure;
using BookingWebAPI.Middleware;
using BookingWebAPI.Services;
using BookingWebAPI.TaskManagement;
using BookingWebAPI.Utils;
using Microsoft.EntityFrameworkCore;

var builder = WebApplication.CreateBuilder(args);

// If the app is not locally hosted: use Key Vault config.
//if(Environment.GetEnvironmentVariable("ASPNETCORE_ENVIRONMENT") != Environments.Development)
//{
//    string? keyVaultUri = Environment.GetEnvironmentVariable("AZURE_KEYVAULT_URI");
//    if(keyVaultUri != null)
//    {
//        builder.Configuration.AddAzureKeyVault(new Uri(keyVaultUri), new DefaultAzureCredential());
//    }
//}

var apiConnectionString = builder.Configuration.GetConnectionString("DefaultDatabaseConnection");

// Add services to the container.
builder.Services.AddDALRegistrations(apiConnectionString)
    .AddServicesRegistrations()
    .AddInfrastructureRegistrations();

builder.Services.AddControllers();

// Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle
builder.Services.AddEndpointsApiExplorer()
    .AddSwaggerGen(SwaggerSecurityDefinition.Add);

builder.Services.ConfigureHangfire(apiConnectionString);

builder.Services.Configure<EmailConfiguration>(builder.Configuration.GetSection("EmailConfig"))
    .Configure<JwtConfiguration>(builder.Configuration.GetSection("JwtConfig"))
    .Configure<FrontEndConfiguration>(builder.Configuration.GetSection("FrontEndConfig"))
    .Configure<BackEndConfiguration>(builder.Configuration.GetSection("BackEndConfig"));

var corsPolicy = new CorsPolicyConfiguration();
builder.Configuration.GetSection("CorsPolicy").Bind(corsPolicy);
// TODO: more restricitive CORS
builder.Services.AddCors(options => options.AddDefaultPolicy(b => b.WithOrigins(corsPolicy.AllowedOrigins).AllowAnyHeader().AllowAnyMethod().AllowCredentials() ));

var app = builder.Build();

if (!app.Environment.IsProduction())
{
    using (var scope = app.Services.CreateScope())
    {
        try
        {
            using var dbContext = scope.ServiceProvider.GetRequiredService<BookingWebAPIDbContext>();
            dbContext.Database.Migrate();
            dbContext.Seed();
        }
        catch (Exception e)
        {
            var logger = scope.ServiceProvider.GetRequiredService<ILogger<Program>>();
            logger.LogError(e, "An error occurred during database seeding at startup.");
        }
    }
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.UseExceptionHandler(exceptionHandlerApp => exceptionHandlerApp.UseMiddleware<BookingWebAPIExceptionHandler>())
    .UseHttpsRedirection()
    .UseCors()
    .UseMiddleware<JwtAuthMiddleware>()
    .UseAuthorization();

// Turns on attribute routing (but NOT conventional routing)
app.MapControllers();

app.Run();
