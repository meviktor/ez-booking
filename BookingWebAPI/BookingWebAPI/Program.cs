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

app.UseAuthorization();

app.MapControllers();

app.Run();
