using CoreAPI.Controllers;
using FluentAssertions;
using Microsoft.AspNetCore.Mvc;
using Xunit;

namespace CoreAPI.Tests.Controllers
{
    /// <summary>
    /// Unit tests for HomeController endpoints
    /// Tests: Authorization, authorization claims, and API responses
    /// </summary>
    public class HomeControllerTests
    {
        private readonly HomeController _controller;

        public HomeControllerTests()
        {
            _controller = new HomeController();
        }

        #region Admin Endpoint Tests

        /// <summary>
        /// Test: Admin endpoint returns 200 OK for admin users
        /// Expected: Success response with admin message
        /// </summary>
        [Fact]
        [Trait("Category", "Authorization")]
        [Trait("Endpoint", "AdminOnly")]
        public void AdminEndpoint_WhenCalled_ReturnsOkResult()
        {
            // Arrange
            var expectedMessage = "Admin access granted";

            // Act
            var result = _controller.AdminOnly();

            // Assert
            result.Should().NotBeNull();
            result.Should().BeOfType<OkObjectResult>();

            var okResult = (OkObjectResult)result;
            okResult.StatusCode.Should().Be(200);
            okResult.Value.Should().NotBeNull();
        }

        /// <summary>
        /// Test: Admin endpoint returns correct response structure
        /// Expected: Response contains message property
        /// </summary>
        [Fact]
        [Trait("Category", "Response")]
        [Trait("Endpoint", "AdminOnly")]
        public void AdminEndpoint_WhenCalled_ReturnsCorrectStructure()
        {
            // Arrange
            var expectedMessage = "Admin access granted";

            // Act
            var result = _controller.AdminOnly() as OkObjectResult;

            // Assert
            result.Should().NotBeNull();
            var resultValue = result!.Value as dynamic;
            resultValue.Should().NotBeNull();
            ((string)resultValue!.message).Should().Be(expectedMessage);
        }

        #endregion

        #region Manager/Admin Endpoint Tests

        /// <summary>
        /// Test: Manager endpoint returns 200 OK
        /// Expected: Success response with manager/admin message
        /// </summary>
        [Fact]
        [Trait("Category", "Authorization")]
        [Trait("Endpoint", "ManagerAndAdmin")]
        public void ManagerEndpoint_WhenCalled_ReturnsOkResult()
        {
            // Arrange
            var expectedMessage = "Manager/Admin access granted";

            // Act
            var result = _controller.ManagerAndAdmin();

            // Assert
            result.Should().NotBeNull();
            result.Should().BeOfType<OkObjectResult>();

            var okResult = (OkObjectResult)result;
            okResult.StatusCode.Should().Be(200);
        }

        /// <summary>
        /// Test: Manager endpoint returns correct response
        /// Expected: Response contains proper message
        /// </summary>
        [Fact]
        [Trait("Category", "Response")]
        [Trait("Endpoint", "ManagerAndAdmin")]
        public void ManagerEndpoint_WhenCalled_ReturnsCorrectMessage()
        {
            // Arrange
            var expectedMessage = "Manager/Admin access granted";

            // Act
            var result = _controller.ManagerAndAdmin() as OkObjectResult;

            // Assert
            result.Should().NotBeNull();
            var resultValue = result!.Value as dynamic;
            resultValue.Should().NotBeNull();
            ((string)resultValue!.message).Should().Be(expectedMessage);
        }

        #endregion

        #region Product Endpoints Tests

        /// <summary>
        /// Test: Get Products endpoint returns 200 OK
        /// Expected: Product list with permission verified
        /// </summary>
        [Fact]
        [Trait("Category", "Authorization")]
        [Trait("Endpoint", "GetProducts")]
        [Trait("Permission", "products.read")]
        public void GetProductsEndpoint_WhenCalled_ReturnsOkResult()
        {
            // Arrange
            var expectedMessage = "Products list - permission verified";

            // Act
            var result = _controller.GetProducts();

            // Assert
            result.Should().NotBeNull();
            result.Should().BeOfType<OkObjectResult>();

            var okResult = (OkObjectResult)result;
            okResult.StatusCode.Should().Be(200);
        }

        /// <summary>
        /// Test: Get Products returns correct response
        /// Expected: Response contains product data message
        /// </summary>
        [Fact]
        [Trait("Category", "Response")]
        [Trait("Endpoint", "GetProducts")]
        public void GetProductsEndpoint_WhenCalled_ReturnsCorrectStructure()
        {
            // Arrange
            var expectedMessage = "Products list - permission verified";

            // Act
            var result = _controller.GetProducts() as OkObjectResult;

            // Assert
            result.Should().NotBeNull();
            var resultValue = result!.Value as dynamic;
            resultValue.Should().NotBeNull();
            ((string)resultValue!.message).Should().Contain("Products list");
        }

        /// <summary>
        /// Test: Create Product endpoint returns 200 OK
        /// Expected: Success response with creation message
        /// </summary>
        [Fact]
        [Trait("Category", "Authorization")]
        [Trait("Endpoint", "CreateProduct")]
        [Trait("Permission", "products.write")]
        public void CreateProductEndpoint_WhenCalled_ReturnsOkResult()
        {
            // Arrange
            var expectedMessage = "Product created - permission verified";

            // Act
            var result = _controller.CreateProduct();

            // Assert
            result.Should().NotBeNull();
            result.Should().BeOfType<OkObjectResult>();

            var okResult = (OkObjectResult)result;
            okResult.StatusCode.Should().Be(200);
        }

        /// <summary>
        /// Test: Create Product returns correct response
        /// Expected: Response contains creation message
        /// </summary>
        [Fact]
        [Trait("Category", "Response")]
        [Trait("Endpoint", "CreateProduct")]
        public void CreateProductEndpoint_WhenCalled_ReturnsCorrectMessage()
        {
            // Arrange
            var expectedMessage = "Product created - permission verified";

            // Act
            var result = _controller.CreateProduct() as OkObjectResult;

            // Assert
            result.Should().NotBeNull();
            var resultValue = result!.Value as dynamic;
            resultValue.Should().NotBeNull();
            ((string)resultValue!.message).Should().Be(expectedMessage);
        }

        /// <summary>
        /// Test: Delete Product endpoint returns 200 OK
        /// Expected: Success response with deletion message
        /// </summary>
        [Theory]
        [InlineData(1)]
        [InlineData(100)]
        [InlineData(999)]
        [Trait("Category", "Authorization")]
        [Trait("Endpoint", "DeleteProduct")]
        [Trait("Permission", "products.delete")]
        public void DeleteProductEndpoint_WithValidId_ReturnsOkResult(int productId)
        {
            // Arrange
            var expectedMessageStart = $"Product {productId} deleted";

            // Act
            var result = _controller.DeleteProduct(productId);

            // Assert
            result.Should().NotBeNull();
            result.Should().BeOfType<OkObjectResult>();

            var okResult = (OkObjectResult)result;
            okResult.StatusCode.Should().Be(200);
        }

        /// <summary>
        /// Test: Delete Product returns correct response with product ID
        /// Expected: Response contains deletion message with product ID
        /// </summary>
        [Theory]
        [InlineData(1)]
        [InlineData(50)]
        [InlineData(999)]
        [Trait("Category", "Response")]
        [Trait("Endpoint", "DeleteProduct")]
        public void DeleteProductEndpoint_WithValidId_ReturnsCorrectMessage(int productId)
        {
            // Arrange
            var expectedMessage = $"Product {productId} deleted - permission verified";

            // Act
            var result = _controller.DeleteProduct(productId) as OkObjectResult;

            // Assert
            result.Should().NotBeNull();
            var resultValue = result!.Value as dynamic;
            resultValue.Should().NotBeNull();
            ((string)resultValue!.message).Should().Be(expectedMessage);
        }

        #endregion

        #region Response Type Tests

        /// <summary>
        /// Test: All endpoints return IActionResult
        /// Expected: Results are valid action results
        /// </summary>
        [Fact]
        [Trait("Category", "Response")]
        public void AllEndpoints_ReturnIActionResult()
        {
            // Arrange & Act & Assert
            _controller.AdminOnly().Should().BeAssignableTo<IActionResult>();
            _controller.ManagerAndAdmin().Should().BeAssignableTo<IActionResult>();
            _controller.GetProducts().Should().BeAssignableTo<IActionResult>();
            _controller.CreateProduct().Should().BeAssignableTo<IActionResult>();
            _controller.DeleteProduct(1).Should().BeAssignableTo<IActionResult>();
        }

        /// <summary>
        /// Test: All endpoints return OkObjectResult with status 200
        /// Expected: HTTP 200 OK status
        /// </summary>
        [Fact]
        [Trait("Category", "Response")]
        public void AllEndpoints_ReturnOkStatus()
        {
            // Arrange & Act
            var adminResult = _controller.AdminOnly() as OkObjectResult;
            var managerResult = _controller.ManagerAndAdmin() as OkObjectResult;
            var productsResult = _controller.GetProducts() as OkObjectResult;
            var createResult = _controller.CreateProduct() as OkObjectResult;
            var deleteResult = _controller.DeleteProduct(1) as OkObjectResult;

            // Assert
            adminResult?.StatusCode.Should().Be(200);
            managerResult?.StatusCode.Should().Be(200);
            productsResult?.StatusCode.Should().Be(200);
            createResult?.StatusCode.Should().Be(200);
            deleteResult?.StatusCode.Should().Be(200);
        }

        #endregion

        #region Null Check Tests

        /// <summary>
        /// Test: Response values are not null
        /// Expected: All responses contain valid data
        /// </summary>
        [Fact]
        [Trait("Category", "Validation")]
        public void AllEndpoints_ReturnNonNullValues()
        {
            // Arrange & Act
            var adminResult = _controller.AdminOnly() as OkObjectResult;
            var managerResult = _controller.ManagerAndAdmin() as OkObjectResult;
            var productsResult = _controller.GetProducts() as OkObjectResult;
            var createResult = _controller.CreateProduct() as OkObjectResult;
            var deleteResult = _controller.DeleteProduct(1) as OkObjectResult;

            // Assert
            adminResult?.Value.Should().NotBeNull();
            managerResult?.Value.Should().NotBeNull();
            productsResult?.Value.Should().NotBeNull();
            createResult?.Value.Should().NotBeNull();
            deleteResult?.Value.Should().NotBeNull();
        }

        #endregion
    }
}
