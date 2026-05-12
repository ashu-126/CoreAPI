using CoreAPI.Data.Context;
using CoreAPI.Data.Models;
using CoreAPI.IRepository;
using Microsoft.EntityFrameworkCore;
using Microsoft.IdentityModel.Tokens;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;

namespace CoreAPI.Repository
{
    public class JWTRepository: IJWTRepository
    {
        private readonly IConfiguration _config;
        private readonly Context _db;
        public JWTRepository(IConfiguration config, Context db)
        {
            _config = config;
            _db = db;
        }
        // ── PRIVATE: JWT generation ────────────────────────────────────────
        public string GenerateAccessToken(AspNetUser user, IList<string> roles)
        {
            var jwt = _config.GetSection("JwtSettings");

            // Null-safe reads with fallbacks
            var secretKey = jwt["SecretKey"]
                             ?? throw new InvalidOperationException("JwtSettings:SecretKey is not configured.");
            var issuer = jwt["Issuer"]
                             ?? throw new InvalidOperationException("JwtSettings:Issuer is not configured.");
            var audience = jwt["Audience"]
                             ?? throw new InvalidOperationException("JwtSettings:Audience is not configured.");
            var expiryMins = double.TryParse(jwt["AccessTokenExpiryMinutes"], out var mins) ? mins : 60;

            var key = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(secretKey));

            var roleIds = _db.AspNetUsers
                .Where(u => u.Id == user.Id)
                .SelectMany(u => u.Roles)
                .Select(r => r.Id)
                .ToList();

            var permissions = _db.PolicyRoles
            .Where(pr => roleIds.Contains(pr.RoleId))
            .Include(pr => pr.Policy)
            .Select(pr => pr.Policy.Name)
            .Distinct()
            .ToList();

            var claims = new List<Claim>
        {
            new(JwtRegisteredClaimNames.Sub,   user.Id),
            new(JwtRegisteredClaimNames.Email, user.Email!),
            new(JwtRegisteredClaimNames.Jti,   Guid.NewGuid().ToString()),
            new(ClaimTypes.Name,               user.FullName)
        };
            claims.AddRange(roles.Select(r => new Claim(ClaimTypes.Role, r)));
            claims.AddRange(permissions.Select(p => new Claim("permission", p)));


            var token = new JwtSecurityToken(
                issuer: jwt["Issuer"],
                audience: jwt["Audience"],
                claims: claims,
                expires: DateTime.UtcNow.AddMinutes(
                                        double.Parse(jwt["AccessTokenExpiryMinutes"]!)),
                signingCredentials: new SigningCredentials(key, SecurityAlgorithms.HmacSha256)
            );

            return new JwtSecurityTokenHandler().WriteToken(token);
        }
    }
}
