using BookingWebAPI.Common.Constants;
using BookingWebAPI.DAL.Infrastructure;
using BookingWebAPI.DAL.Interfaces;
using BookingWebAPI.DAL.Repositories;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Storage;
using Microsoft.Extensions.DependencyInjection;

namespace BookingWebAPI.DAL
{
    public static class SetupDALInfrastructure
    {
        /// <summary>
        /// Adds the registrations for types being situated in the DAL layer and the database context.
        /// </summary>
        public static IServiceCollection AddDALRegistrations(this IServiceCollection services, string? connectionString)
        {
            // Add registration of interface-implementation pairs here
            services.AddScoped<IUserRepository, UserRepository>();
            services.AddScoped<IResourceCategoryRepository, ResourceCategoryRepository>();
            services.AddScoped<IResourceRepository, ResourceRepository>();
            services.AddScoped<ISiteRepository, SiteRepository>();
            services.AddScoped<ISettingRepository, SettingRepository>();
            services.AddScoped<IEmailConfirmationAttemptRepository, EmailConfirmationAttemptRepository>();
            services.AddScoped<IDbContextTransactionManager, BookingWebAPITransactionManager>();

            if (connectionString is null)
            {
                throw new ArgumentNullException(nameof(connectionString), ApplicationConstants.AppStartupErrorNoConnectionString);
            }
            // Here in the lambda we "configure" what DbContextOptionsBuilder object will be passed to the db context's constructor.
            // Other possible option would be call this without any arguments and configure the context (to use SQL Server as db provider/engine, specify connection string...)
            // int its OnConfigure() method. 
            return services.AddDbContext<BookingWebAPIDbContext>(options => options.UseSqlServer(connectionString));
        }
    }
}
