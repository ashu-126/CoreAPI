using Microsoft.AspNetCore.Authorization;

namespace CoreAPI.Authorization
{
    public class PermissionHandler : AuthorizationHandler<PermissionRequirement>
    {
        protected override Task HandleRequirementAsync(
            AuthorizationHandlerContext context,
            PermissionRequirement requirement)
        {
            // Check permission claims embedded in JWT
            var hasPermission = context.User.Claims
                .Any(c => c.Type == "permission" && c.Value == requirement.Permission);

            if (hasPermission)
                context.Succeed(requirement);

            return Task.CompletedTask;
        }
    }
}
