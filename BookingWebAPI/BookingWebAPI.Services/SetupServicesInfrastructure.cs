﻿using BookingWebAPI.DAL.Interfaces;
using BookingWebAPI.Services.Interfaces;
using Microsoft.Extensions.DependencyInjection;

namespace BookingWebAPI.Services
{
    public static class SetupServicesInfrastructure
    {
        /// <summary>
        /// Adds the registrations for types being situated in the Services layer.
        /// </summary>
        public static IServiceCollection AddServicesRegistrations(this IServiceCollection services)
        {
            services.AddScoped<IResourceCategoryService, ResourceCategoryService>();
            services.AddScoped<IUserService, UserService>();
            services.AddScoped<ISettingService, SettingService>();
            services.AddScoped<IEmailService, EmailService>();
            services.AddScoped<IEmailConfirmationAttemptService, EmailConfirmationAttemptService>();
            return services;
        }
    }
}