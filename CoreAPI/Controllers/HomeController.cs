using CoreAPI.Authorization;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace CoreAPI.Controllers
{
    /// <summary>
    /// Home controller providing role-based and permission-based API endpoints
    /// All endpoints require authentication via [Authorize] attribute
    /// </summary>
    [Route("api/[controller]")]
    [ApiController]
    [Authorize]
    public class HomeController : ControllerBase
    {
        /// <summary>
        /// Admin only endpoint
        /// Requires: Admin role
        /// Returns: Success message
        /// </summary>
        /// <returns>Admin access confirmation</returns>
        [HttpGet("admin")]
        [Authorize(Roles = "Admin")]
        public IActionResult AdminOnly()
        {
            return Ok(new { message = "Admin access granted" });
        }

        /// <summary>
        /// Manager and Admin endpoint
        /// Requires: Admin or Manager role
        /// Returns: Success message
        /// </summary>
        /// <returns>Manager/Admin access confirmation</returns>
        [HttpGet("manager")]
        [Authorize(Roles = "Admin,Manager")]
        public IActionResult ManagerAndAdmin()
        {
            return Ok(new { message = "Manager/Admin access granted" });
        }

        /// <summary>
        /// Get products endpoint
        /// Requires: products.read permission
        /// Returns: Products list confirmation
        /// </summary>
        /// <returns>Products list with permission verification</returns>
        [HttpGet("products")]
        [RequirePermission("products.read")]
        public IActionResult GetProducts()
        {
            return Ok(new { message = "Products list - permission verified" });
        }

        /// <summary>
        /// Create product endpoint
        /// Requires: products.write permission
        /// Returns: Creation confirmation
        /// </summary>
        /// <returns>Product creation confirmation</returns>
        [HttpPost("products")]
        [RequirePermission("products.write")]
        public IActionResult CreateProduct()
        {
            return Ok(new { message = "Product created - permission verified" });
        }

        /// <summary>
        /// Delete product endpoint
        /// Requires: products.delete permission
        /// </summary>
        /// <param name="id">Product identifier to delete</param>
        /// <returns>Product deletion confirmation</returns>
        [HttpDelete("products/{id}")]
        [RequirePermission("products.delete")]
        public IActionResult DeleteProduct(int id)
        {
            if (id <= 0)
            {
                return BadRequest(new { error = "Product ID must be greater than zero" });
            }

            return Ok(new { message = $"Product {id} deleted - permission verified" });
        }
    }
}

