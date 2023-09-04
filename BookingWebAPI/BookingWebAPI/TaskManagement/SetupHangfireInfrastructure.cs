using BookingWebAPI.Common.Constants;
using BookingWebAPI.DAL.Interfaces;
using Hangfire;

namespace BookingWebAPI.TaskManagement
{
    public static class SetupHangfireInfrastructure
    {
        public static void ConfigureHangfire(this IServiceCollection services, string? connectionString)
        {
            if(connectionString == null)
            {
                throw new ArgumentNullException(nameof(connectionString), ApplicationConstants.AppStartupErrorNoConnectionString);
            }
            services.AddHangfire(configuration => configuration
                .SetDataCompatibilityLevel(CompatibilityLevel.Version_180)
                .UseSimpleAssemblyNameTypeSerializer()
                .UseRecommendedSerializerSettings()
                .UseSqlServerStorage(connectionString));

            services.AddHangfireServer();
            services.AddScoped<IBackgroundJobClient, BackgroundJobClient>();
        }
    }
}
