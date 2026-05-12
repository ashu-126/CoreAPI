using CoreAPI.Controllers;
using FluentAssertions;
using Microsoft.AspNetCore.Mvc;
using System.Dynamic;
using Xunit;

namespace CoreAPI.Tests.Response
{
    /// <summary>
    /// Tests for response validation
    /// Verifies: Response format, content, and data structures
    /// </summary>
    public class ResponseValidationTests
    {
        private readonly HomeController _controller;

        public ResponseValidationTests()
        {
            _controller = new HomeController();
        }

        #region Response Format Tests

        /// <summary>
        /// Test: All responses contain required message property
        /// Expected: Message property exists and is not empty
        /// </summary>
        [Fact]
        [Trait("Category", "Response")]
        public void AllResponses_ContainMessageProperty()
        {
            // Act
            var adminResponse = ExtractResponseValue(_controller.AdminOnly());
            var managerResponse = ExtractResponseValue(_controller.ManagerAndAdmin());
            var productsResponse = ExtractResponseValue(_controller.GetProducts());
            var createResponse = ExtractResponseValue(_controller.CreateProduct());
            var deleteResponse = ExtractResponseValue(_controller.DeleteProduct(1));

            // Assert
            adminResponse.Should().NotBeNull().And.HaveProperty("message");
            managerResponse.Should().NotBeNull().And.HaveProperty("message");
            productsResponse.Should().NotBeNull().And.HaveProperty("message");
            createResponse.Should().NotBeNull().And.HaveProperty("message");
            deleteResponse.Should().NotBeNull().And.HaveProperty("message");
        }

        /// <summary>
        /// Test: Response message properties are not empty
        /// Expected: All messages have content
        /// </summary>
        [Fact]
        [Trait("Category", "Response")]
        public void ResponseMessages_AreNotEmpty()
        {
            // Act
            var adminMessage = GetMessageFromResponse(_controller.AdminOnly());
            var managerMessage = GetMessageFromResponse(_controller.ManagerAndAdmin());
            var productsMessage = GetMessageFromResponse(_controller.GetProducts());
            var createMessage = GetMessageFromResponse(_controller.CreateProduct());
            var deleteMessage = GetMessageFromResponse(_controller.DeleteProduct(1));

            // Assert
            adminMessage.Should().NotBeNullOrWhiteSpace();
            managerMessage.Should().NotBeNullOrWhiteSpace();
            productsMessage.Should().NotBeNullOrWhiteSpace();
            createMessage.Should().NotBeNullOrWhiteSpace();
            deleteMessage.Should().NotBeNullOrWhiteSpace();
        }

        #endregion

        #region Message Content Tests

        /// <summary>
        /// Test: Admin endpoint message contains expected text
        /// Expected: Message mentions "Admin" and "access granted"
        /// </summary>
        [Fact]
        [Trait("Category", "Response")]
        public void AdminResponse_ContainsExpectedMessage()
        {
            // Act
            var message = GetMessageFromResponse(_controller.AdminOnly());

            // Assert
            message.Should().Contain("Admin");
            message.Should().Contain("access granted");
        }

        /// <summary>
        /// Test: Manager endpoint message contains expected text
        /// Expected: Message mentions "Manager/Admin"
        /// </summary>
        [Fact]
        [Trait("Category", "Response")]
        public void ManagerResponse_ContainsExpectedMessage()
        {
            // Act
            var message = GetMessageFromResponse(_controller.ManagerAndAdmin());

            // Assert
            message.Should().Contain("Manager");
            message.Should().Contain("Admin");
        }

        /// <summary>
        /// Test: Products endpoint message contains expected text
        /// Expected: Message mentions "Products"
        /// </summary>
        [Fact]
        [Trait("Category", "Response")]
        public void ProductsResponse_ContainsExpectedMessage()
        {
            // Act
            var message = GetMessageFromResponse(_controller.GetProducts());

            // Assert
            message.Should().Contain("Products");
        }

        /// <summary>
        /// Test: Create endpoint message contains expected text
        /// Expected: Message mentions "created" and "permission"
        /// </summary>
        [Fact]
        [Trait("Category", "Response")]
        public void CreateResponse_ContainsExpectedMessage()
        {
            // Act
            var message = GetMessageFromResponse(_controller.CreateProduct());

            // Assert
            message.Should().Contain("created");
            message.Should().Contain("permission");
        }

        /// <summary>
        /// Test: Delete endpoint message contains product ID
        /// Expected: Message includes the product ID
        /// </summary>
        [Theory]
        [InlineData(1)]
        [InlineData(42)]
        [InlineData(999)]
        [Trait("Category", "Response")]
        public void DeleteResponse_ContainsProductId(int productId)
        {
            // Act
            var message = GetMessageFromResponse(_controller.DeleteProduct(productId));

            // Assert
            message.Should().Contain(productId.ToString());
            message.Should().Contain("deleted");
        }

        #endregion

        #region Response Structure Tests

        /// <summary>
        /// Test: Response structure is consistent
        /// Expected: All responses have same structure
        /// </summary>
        [Fact]
        [Trait("Category", "Response")]
        public void ResponseStructure_IsConsistent()
        {
            // Arrange
            var endpoints = new Func<IActionResult>[]
            {
                _controller.AdminOnly,
                _controller.ManagerAndAdmin,
                _controller.GetProducts,
                _controller.CreateProduct,
                () => _controller.DeleteProduct(1)
            };

            // Act & Assert
            foreach (var endpoint in endpoints)
            {
                var result = endpoint();
                result.Should().BeOfType<OkObjectResult>();

                var okResult = (OkObjectResult)result;
                var value = okResult.Value as dynamic;

                value.Should().HaveProperty("message");
                ((string)value?.message).Should().NotBeNullOrEmpty();
            }
        }

        /// <summary>
        /// Test: Response status code is always 200 OK
        /// Expected: Consistent HTTP 200 response
        /// </summary>
        [Fact]
        [Trait("Category", "Response")]
        public void ResponseStatusCode_IsConsistent()
        {
            // Arrange
            var endpoints = new Func<IActionResult>[]
            {
                _controller.AdminOnly,
                _controller.ManagerAndAdmin,
                _controller.GetProducts,
                _controller.CreateProduct,
                () => _controller.DeleteProduct(1)
            };

            // Act & Assert
            foreach (var endpoint in endpoints)
            {
                var result = endpoint() as OkObjectResult;
                result?.StatusCode.Should().Be(200);
            }
        }

        #endregion

        #region Data Type Tests

        /// <summary>
        /// Test: Message properties are strings
        /// Expected: All message values are string type
        /// </summary>
        [Fact]
        [Trait("Category", "Response")]
        public void MessageProperties_AreStrings()
        {
            // Act
            var adminMessage = GetMessageFromResponse(_controller.AdminOnly());
            var managerMessage = GetMessageFromResponse(_controller.ManagerAndAdmin());
            var productsMessage = GetMessageFromResponse(_controller.GetProducts());
            var createMessage = GetMessageFromResponse(_controller.CreateProduct());
            var deleteMessage = GetMessageFromResponse(_controller.DeleteProduct(1));

            // Assert
            adminMessage.Should().BeOfType<string>();
            managerMessage.Should().BeOfType<string>();
            productsMessage.Should().BeOfType<string>();
            createMessage.Should().BeOfType<string>();
            deleteMessage.Should().BeOfType<string>();
        }

        #endregion

        #region Helper Methods

        /// <summary>
        /// Extract response value from IActionResult
        /// </summary>
        private static dynamic? ExtractResponseValue(IActionResult result)
        {
            if (result is OkObjectResult okResult)
            {
                return okResult.Value;
            }

            return null;
        }

        /// <summary>
        /// Get message from response
        /// </summary>
        private static string GetMessageFromResponse(IActionResult result)
        {
            var okResult = result as OkObjectResult;
            var value = okResult?.Value as dynamic;
            return value?.message ?? string.Empty;
        }

        #endregion
    }
}
