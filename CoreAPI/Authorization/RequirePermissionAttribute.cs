using Microsoft.AspNetCore.Authorization;

namespace CoreAPI.Authorization
{
    public class RequirePermissionAttribute : AuthorizeAttribute
    {
        public RequirePermissionAttribute(string permission)
            : base(policy: $"Permission:{permission}") { }
    }
}
