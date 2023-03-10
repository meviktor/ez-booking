using BookingWebAPI.DAL.Interfaces;
using BookingWebAPI.DAL.Repositories;
using Microsoft.EntityFrameworkCore;
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

            if (connectionString is null)
            {
                throw new ArgumentNullException(nameof(connectionString), "No connection string provided for database access. Check your configuration for missing connection string.");
            }
            return services.AddDbContext<BookingWebAPIDbContext>(options => options.UseSqlServer(connectionString));
        }
    }
}
