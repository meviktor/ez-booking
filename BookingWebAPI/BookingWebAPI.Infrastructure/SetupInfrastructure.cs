using BookingWebAPI.Infrastructure.ViewModels.Mapping;
using Microsoft.Extensions.DependencyInjection;

namespace BookingWebAPI.Infrastructure
{
    public static class SetupInfrastructure
    {
        /// <summary>
        /// Adds the registrations for types of the infrastructure project.
        /// </summary>
        public static IServiceCollection AddInfrastructureRegistrations(this IServiceCollection services)
        {
            services.AddAutoMapper(typeof(ViewModelProfile));
            return services;
        }
    }
}
