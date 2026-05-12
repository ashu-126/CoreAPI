using CoreAPI.Authorization;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace CoreAPI.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    [Authorize]
    public class HomeController : ControllerBase
    {
        [HttpGet("admin")]
        [Authorize(Roles = "Admin")]
        public IActionResult AdminOnly()
        {
            return Ok(new { message = "Admin access granted" });
        }

        [HttpGet("manager")]
        [Authorize(Roles = "Admin,Manager")]
        public IActionResult ManagerAndAdmin()
        {
            return Ok(new { message = "Manager/Admin access granted" });
        }
        // ✅ Fine-grained permission from DB (dynamic)
        [HttpGet("products")]
        [RequirePermission("products.read")]
        public IActionResult GetProducts()
        {
            return Ok(new { message = "Products list - permission verified" });
        }

        [HttpPost("products")]
        [RequirePermission("products.write")]
        public IActionResult CreateProduct()
        {
            return Ok(new { message = "Product created - permission verified" });
        }

        [HttpDelete("products/{id}")]
        [RequirePermission("products.delete")]
        public IActionResult DeleteProduct(int id)
        {
            return Ok(new { message = $"Product {id} deleted - permission verified" });
        }
    }
}
