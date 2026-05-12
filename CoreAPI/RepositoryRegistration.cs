using CoreAPI.Authorization;
using CoreAPI.Data.Models;
using CoreAPI.IRepository;
using CoreAPI.Repository;
using Microsoft.AspNetCore.Authorization;

namespace CoreAPI
{
    public static class RepositoryRegistration
    {
        public static IServiceCollection AddRepositories(this IServiceCollection services)
        {
            services.AddMemoryCache();
            // register your other services/repos here

            // --- Dynamic Authorization ---
            services.AddSingleton<IAuthorizationPolicyProvider, DynamicPermissionPolicyProvider>();
            services.AddScoped<IAuthorizationHandler, PermissionHandler>();

            // services.AddScoped<IAuthService, AuthService>();
            services.AddScoped<IAuthRepository, AuthRepository>();
            services.AddScoped<IJWTRepository, JWTRepository>();

            return services;
        }
    }
}
