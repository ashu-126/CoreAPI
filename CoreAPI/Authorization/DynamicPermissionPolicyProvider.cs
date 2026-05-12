using Microsoft.AspNetCore.Authorization;
using Microsoft.Extensions.Options;

namespace CoreAPI.Authorization
{
    public class DynamicPermissionPolicyProvider : IAuthorizationPolicyProvider
    {
        private const string PolicyPrefix = "Permission:";
        private readonly DefaultAuthorizationPolicyProvider _fallback;

        public DynamicPermissionPolicyProvider(IOptions<AuthorizationOptions> options)
            => _fallback = new DefaultAuthorizationPolicyProvider(options);

        public Task<AuthorizationPolicy> GetDefaultPolicyAsync() =>
            _fallback.GetDefaultPolicyAsync();

        public Task<AuthorizationPolicy?> GetFallbackPolicyAsync() =>
            _fallback.GetFallbackPolicyAsync();

        public Task<AuthorizationPolicy?> GetPolicyAsync(string policyName)
        {
            if (policyName.StartsWith(PolicyPrefix, StringComparison.OrdinalIgnoreCase))
            {
                var permission = policyName[PolicyPrefix.Length..];
                var policy = new AuthorizationPolicyBuilder()
                    .RequireAuthenticatedUser()
                    .AddRequirements(new PermissionRequirement(permission))
                    .Build();

                return Task.FromResult<AuthorizationPolicy?>(policy);
            }

            return _fallback.GetPolicyAsync(policyName);
        }
    }
}
