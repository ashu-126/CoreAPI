namespace CoreAPI.Models
{
    public class AuthModel
    {
    }
    public class LoginRequest
    {
        public string Email { get; set; } = null!;
        public string Password { get; set; } = null!;
    }
    public class RegisterRequest
    {
        public string FullName { get; set; } = null!;
        public string Email { get; set; } = null!;
        public string Password { get; set; } = null!;
        public string Role { get; set; } = null!;
    }
    public class AuthResponse
    {

        public AuthResponse(string accessToken, string token, string fullName, IList<string> roles)
        {
            AccessToken = accessToken;
            this.token = token;
            this.fullName = fullName;
            Roles = roles;
        }

        private string token;
        private string fullName;
        public string AccessToken { get; set; }
        public string RefreshToken { get; set; }
        public string Username { get; set; }
        public IList<string> Roles { get; set; }
    }
}
