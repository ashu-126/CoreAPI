using CoreAPI.IRepository;
using CoreAPI.Models;
using Microsoft.AspNetCore.Mvc;

namespace CoreAPI.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class AuthController : ControllerBase
    {
        private readonly IAuthRepository _auth;
        private string IpAddress =>
        HttpContext.Connection.RemoteIpAddress?.ToString() ?? "unknown";
        public AuthController(IAuthRepository auth)
        {
            _auth = auth;
        }

        [HttpPost("login")]
        public async Task<IActionResult> Login([FromBody] LoginRequest request)
        {
            var result = await _auth.LoginAsync(request, IpAddress);
            if (result == null)
                return Unauthorized(new { message = "Invalid email or password." });
            return Ok(result);
        }

        [HttpPost("register")]
        public async Task<IActionResult> Register(RegisterRequest req)
        {
            try
            {
                var role = await _auth.RegisterAsync(req);
                return Ok(new { message = "Registered successfully", role });
            }
            catch (Exception ex)
            {
                return BadRequest(new { message = ex.Message });
            }
        }
    }
}
