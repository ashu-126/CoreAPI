using CoreAPI.Data.Context;
using CoreAPI.Data.Models;
using CoreAPI.IRepository;
using CoreAPI.Models;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Caching.Memory;
using System.Security.Cryptography;
using System.Text;

namespace CoreAPI.Repository
{
    public class AuthRepository : IAuthRepository
    {
        private readonly Context _db;
        private readonly IJWTRepository _jwt;
        private readonly IConfiguration _config;
        private readonly IMemoryCache _cache;
        public AuthRepository(Context db, IJWTRepository jwt, IConfiguration config, IMemoryCache cache)
        {
            _db = db;
            _jwt = jwt;
            _config = config;
            _cache = cache;
        }
        public async Task<IList<string>> GetRolesAsync()
        {
            var roles = await _db.AspNetRoles.Select(r => r.Name!).ToListAsync();
            return roles;
        }

        public async Task<AuthResponse?> LoginAsync(LoginRequest request, string ipAddress)
        {
            //ashish@123
            var user = await _db.AspNetUsers.FirstOrDefaultAsync(u => u.NormalizedEmail == request.Email.ToUpper() && u.IsActive == true);

            //if (user == null || !VerifyPassword(request.Password, user.PasswordHash))
            if (user == null)
            {
                return null;
            }

            // Check lockout
            if (user.LockoutEnabled && user.LockoutEnd.HasValue && user.LockoutEnd > DateTimeOffset.UtcNow)
                throw new Exception("Account is locked. Try again later.");

            var roles = await GetUserRolesAsync(user.Id);
            var accessToken = _jwt.GenerateAccessToken(user, roles);
            var refresh = await CreateRefreshTokenAsync(user.Id, ipAddress);

            // Reset failed count on success
            user.AccessFailedCount = 0;
            await _db.SaveChangesAsync();

            await LogAuditAsync(user.Id, "Login", ipAddress);
            return new AuthResponse(accessToken, refresh.Token, user.FullName, roles);

        }

        public async Task<AuthResponse?> RefreshTokenAsync(string token, string ipAddress)
        {
            var stored = await _db.RefreshTokens
            .Include(t => t.User)
            .FirstOrDefaultAsync(t => t.Token == token);

            //if (stored is null || !stored.IsActive) return null;
            if (stored is null) return null;

            // Rotate token
            stored.RevokedAt = DateTime.UtcNow;
            var newRefresh = await CreateRefreshTokenAsync(stored.UserId, ipAddress);
            stored.ReplacedBy = newRefresh.Token;
            await _db.SaveChangesAsync();

            var roles = await GetUserRolesAsync(stored.UserId);
            var accessToken =  _jwt.GenerateAccessToken(stored.User!,roles);

            await LogAuditAsync(stored.UserId, "TokenRefresh", ipAddress);
            return new AuthResponse(accessToken, newRefresh.Token, stored.User!.FullName, roles);
        }

        public async Task<string> RegisterAsync(RegisterRequest req)
        {
            // Check duplicate email
            if (await _db.AspNetUsers.AnyAsync(u => u.NormalizedEmail == req.Email.ToUpper()))
                throw new Exception("Email already registered.");

            // Validate role exists in DB (fully dynamic)
            var validRoles = await _db.AspNetRoles.Select(r => r.Name!).ToListAsync();
            var assignedRole = validRoles.Contains(req.Role) ? req.Role : validRoles.First(r => r == "User");

            var user = new AspNetUser
            {
                Id = Guid.NewGuid().ToString(),
                FullName = req.FullName,
                UserName = req.Email,
                NormalizedUserName = req.Email.ToUpper(),
                Email = req.Email,
                NormalizedEmail = req.Email.ToUpper(),
                PasswordHash = HashPassword(req.Password),
                SecurityStamp = Guid.NewGuid().ToString(),
                ConcurrencyStamp = Guid.NewGuid().ToString(),
                EmailConfirmed = true,
                LockoutEnabled = true,
                IsActive = true,
                CreatedAt = DateTime.UtcNow
            };

            _db.AspNetUsers.Add(user);
            await _db.SaveChangesAsync();

            // Assign role
            var role = await _db.AspNetRoles.FirstAsync(r => r.Name == assignedRole);
            user.Roles.Add(role);
            //_db.AspNetUserRoles.Add(new AspNetUserRole { UserId = user.Id, RoleId = role.Id });
            await _db.SaveChangesAsync();

            return assignedRole;
        }

        public async Task<bool> RevokeTokenAsync(string token)
        {
            var stored = await _db.RefreshTokens
                .FirstOrDefaultAsync(t => t.Token == token && t.RevokedAt == null);

            if (stored is null) return false;

            stored.RevokedAt = DateTime.UtcNow;
            await _db.SaveChangesAsync();
            return true;
        }

        public async Task SeedRolesAsync(IEnumerable<string> roles)
        {
            foreach (var roleName in roles)
            {
                var normalized = roleName.ToUpper();
                if (!await _db.AspNetRoles.AnyAsync(r => r.NormalizedName == normalized))
                {
                    _db.AspNetRoles.Add(new AspNetRole
                    {
                        Id = Guid.NewGuid().ToString(),
                        Name = roleName,
                        NormalizedName = normalized,
                        ConcurrencyStamp = Guid.NewGuid().ToString()
                    });
                }
            }
            await _db.SaveChangesAsync();
        }
        // ── PRIVATE: Password hashing(PBKDF2 — same as ASP.NET Identity) ─
        private static string HashPassword(string password)
        {
            byte[] salt = RandomNumberGenerator.GetBytes(16);
            byte[] hash = Rfc2898DeriveBytes.Pbkdf2(
                Encoding.UTF8.GetBytes(password),
                salt,
                iterations: 10000,
                hashAlgorithm: HashAlgorithmName.SHA256,
                outputLength: 32);

            // Store as: Base64(salt) + "." + Base64(hash)
            return $"{Convert.ToBase64String(salt)}.{Convert.ToBase64String(hash)}";
        }
        private static bool VerifyPassword(string password, string storedHash)
        {
            var parts = storedHash.Split('.');
            if (parts.Length != 2) return false;

            byte[] salt = Convert.FromBase64String(parts[0]);
            byte[] expectedHash = Convert.FromBase64String(parts[1]);

            byte[] actualHash = Rfc2898DeriveBytes.Pbkdf2(
                Encoding.UTF8.GetBytes(password),
                salt,
                iterations: 10000,
                hashAlgorithm: HashAlgorithmName.SHA256,
                outputLength: 32);

            return CryptographicOperations.FixedTimeEquals(actualHash, expectedHash);
        }
        // ── PRIVATE: Get user roles ────────────────────────────────────────
        private async Task<IList<string>> GetUserRolesAsync(string userId)
        {
            var cacheKey = $"user_roles_{userId}";

            if (_cache.TryGetValue(cacheKey, out IList<string>? cached))
                return cached!;

            var roles = await _db.AspNetUsers
                .Where(u => u.Id == userId)
                .SelectMany(u => u.Roles)
                .Select(r => r.Name!)
                .ToListAsync();

            _cache.Set(cacheKey, roles, TimeSpan.FromMinutes(5));
            return roles;
        }

        // ── PRIVATE: Refresh token creation ───────────────────────────────
        private async Task<RefreshToken> CreateRefreshTokenAsync(string userId, string ipAddress)
        {
            var expiryDays = int.Parse(_config["JwtSettings:RefreshTokenExpiryDays"]!);
            var token = new RefreshToken
            {
                UserId = userId,
                Token = Convert.ToBase64String(RandomNumberGenerator.GetBytes(64)),
                ExpiresAt = DateTime.UtcNow.AddDays(expiryDays),
                CreatedByIp = ipAddress
            };
            _db.RefreshTokens.Add(token);
            await _db.SaveChangesAsync();
            return token;
        }
        // ── PRIVATE: Audit logging ─────────────────────────────────────────
        private async Task LogAuditAsync(string userId, string action, string? ip)
        {
            _db.AuditLogs.Add(new AuditLog
            {
                UserId = userId,
                Action = action,
                IpAddress = ip,
                CreatedAt = DateTime.UtcNow
            });
            await _db.SaveChangesAsync();
        }
    }
}
