using BookingWebAPI.Common.Models.Config;
using BookingWebAPI.DAL;
using BookingWebAPI.Infrastructure;
using BookingWebAPI.Middleware;
using BookingWebAPI.Services;
using BookingWebAPI.TaskManagement;
using Microsoft.EntityFrameworkCore;

var builder = WebApplication.CreateBuilder(args);

var apiConnectionString = builder.Configuration.GetConnectionString("DefaultDatabaseConnection");
// Add services to the container.
builder.Services.AddDALRegistrations(apiConnectionString);
builder.Services.AddServicesRegistrations();
builder.Services.AddInfrastructureRegistrations();
builder.Services.AddControllers();

// Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();
builder.Services.ConfigureHangfire(apiConnectionString);
builder.Services.Configure<EmailConfiguration>(builder.Configuration.GetSection("EmailConfig"));
builder.Services.Configure<JwtConfiguration>(builder.Configuration.GetSection("JwtConfig"));
builder.Services.Configure<FrontEndConfiguration>(builder.Configuration.GetSection("FrontEndConfig"));
builder.Services.Configure<BackEndConfiguration>(builder.Configuration.GetSection("BackEndConfig"));

var corsPolicy = new CorsPolicyConfiguration();
builder.Configuration.GetSection("CorsPolicy").Bind(corsPolicy);
builder.Services.AddCors(options => options.AddDefaultPolicy(b => b.WithOrigins(corsPolicy.AllowedOrigins).AllowAnyHeader().AllowAnyMethod().AllowCredentials() ));

var app = builder.Build();

if (app.Environment.IsDevelopment())
{
    using (var scope = app.Services.CreateScope())
    {
        try
        {
            var dbContext = scope.ServiceProvider.GetRequiredService<BookingWebAPIDbContext>();
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

app.UseExceptionHandler(exceptionHandlerApp => exceptionHandlerApp.UseMiddleware<BookingWebAPIExceptionHandler>());

app.UseHttpsRedirection();

app.UseCors();

app.UseMiddleware<JwtAuthMiddleware>();

app.UseAuthorization();

app.MapControllers();

app.Run();
