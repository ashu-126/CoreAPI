

using CoreAPI.Models;

namespace CoreAPI.IRepository
{
    public interface IAuthRepository
    {
        Task<AuthResponse?> LoginAsync(LoginRequest request, string ipAddress);
        Task<string> RegisterAsync(RegisterRequest request);
        Task<AuthResponse?> RefreshTokenAsync(string token, string ipAddress);
        Task<bool> RevokeTokenAsync(string token);
        Task<IList<string>> GetRolesAsync();
        Task SeedRolesAsync(IEnumerable<string> roles);
    }
}
