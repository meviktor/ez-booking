using BookingWebAPI.DAL;
using BookingWebAPI.Infrastructure;
using BookingWebAPI.Middleware;
using BookingWebAPI.Services;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.
builder.Services.AddDALRegistrations(builder.Configuration.GetConnectionString("DefaultDatabaseConnection"));
builder.Services.AddServicesRegistrations();
builder.Services.AddInfrastructureRegistrations();
builder.Services.AddControllers();

// Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

var app = builder.Build();

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.UseExceptionHandler(exceptionHandlerApp => exceptionHandlerApp.UseMiddleware<BookingWebAPIExceptionHandler>());

app.UseHttpsRedirection();

app.UseAuthorization();

app.MapControllers();

app.Run();
