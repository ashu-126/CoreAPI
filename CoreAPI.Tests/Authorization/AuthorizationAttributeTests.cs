using CoreAPI.Authorization;
using FluentAssertions;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System.Reflection;
using Xunit;

namespace CoreAPI.Tests.Authorization
{
    /// <summary>
    /// Tests for authorization attributes and policies
    /// Verifies: Correct attributes on endpoints, required permissions
    /// </summary>
    public class AuthorizationAttributeTests
    {
        private readonly Type _controllerType;

        public AuthorizationAttributeTests()
        {
            _controllerType = typeof(CoreAPI.Controllers.HomeController);
        }

        #region Authorize Attribute Tests

        /// <summary>
        /// Test: Controller has [Authorize] attribute
        /// Expected: Authorization required for all endpoints
        /// </summary>
        [Fact]
        [Trait("Category", "Authorization")]
        public void Controller_HasAuthorizeAttribute()
        {
            // Act
            var hasAuthorizeAttribute = _controllerType
                .GetCustomAttributes(typeof(AuthorizeAttribute), false)
                .Length > 0;

            // Assert
            hasAuthorizeAttribute.Should().BeTrue("Controller should require authorization");
        }

        /// <summary>
        /// Test: AdminOnly endpoint has correct role authorization
        /// Expected: Requires Admin role
        /// </summary>
        [Fact]
        [Trait("Category", "Authorization")]
        [Trait("Endpoint", "AdminOnly")]
        public void AdminOnlyEndpoint_HasCorrectAuthorization()
        {
            // Arrange
            var method = _controllerType.GetMethod("AdminOnly");

            // Act
            var authorizeAttribute = method?
                .GetCustomAttributes(typeof(AuthorizeAttribute), false)
                .Cast<AuthorizeAttribute>()
                .FirstOrDefault();

            // Assert
            authorizeAttribute.Should().NotBeNull("Admin endpoint should have Authorize attribute");
            authorizeAttribute?.Roles.Should().Contain("Admin");
        }

        /// <summary>
        /// Test: ManagerAndAdmin endpoint has correct roles
        /// Expected: Requires Admin or Manager role
        /// </summary>
        [Fact]
        [Trait("Category", "Authorization")]
        [Trait("Endpoint", "ManagerAndAdmin")]
        public void ManagerEndpoint_HasCorrectRoles()
        {
            // Arrange
            var method = _controllerType.GetMethod("ManagerAndAdmin");

            // Act
            var authorizeAttribute = method?
                .GetCustomAttributes(typeof(AuthorizeAttribute), false)
                .Cast<AuthorizeAttribute>()
                .FirstOrDefault();

            // Assert
            authorizeAttribute.Should().NotBeNull("Manager endpoint should have Authorize attribute");
            authorizeAttribute?.Roles.Should().Contain("Admin");
            authorizeAttribute?.Roles.Should().Contain("Manager");
        }

        #endregion

        #region Permission Attribute Tests

        /// <summary>
        /// Test: GetProducts endpoint has permission attribute
        /// Expected: Requires products.read permission
        /// </summary>
        [Fact]
        [Trait("Category", "Authorization")]
        [Trait("Endpoint", "GetProducts")]
        [Trait("Permission", "products.read")]
        public void GetProductsEndpoint_HasPermissionAttribute()
        {
            // Arrange
            var method = _controllerType.GetMethod("GetProducts");

            // Act
            var hasPermissionAttribute = method?
                .GetCustomAttributes(typeof(RequirePermissionAttribute), false)
                .Length > 0;

            // Assert
            hasPermissionAttribute.Should().BeTrue("GetProducts should require permission attribute");
        }

        /// <summary>
        /// Test: CreateProduct endpoint has permission attribute
        /// Expected: Requires products.write permission
        /// </summary>
        [Fact]
        [Trait("Category", "Authorization")]
        [Trait("Endpoint", "CreateProduct")]
        [Trait("Permission", "products.write")]
        public void CreateProductEndpoint_HasPermissionAttribute()
        {
            // Arrange
            var method = _controllerType.GetMethod("CreateProduct");

            // Act
            var hasPermissionAttribute = method?
                .GetCustomAttributes(typeof(RequirePermissionAttribute), false)
                .Length > 0;

            // Assert
            hasPermissionAttribute.Should().BeTrue("CreateProduct should require permission attribute");
        }

        /// <summary>
        /// Test: DeleteProduct endpoint has permission attribute
        /// Expected: Requires products.delete permission
        /// </summary>
        [Fact]
        [Trait("Category", "Authorization")]
        [Trait("Endpoint", "DeleteProduct")]
        [Trait("Permission", "products.delete")]
        public void DeleteProductEndpoint_HasPermissionAttribute()
        {
            // Arrange
            var method = _controllerType.GetMethod("DeleteProduct");

            // Act
            var hasPermissionAttribute = method?
                .GetCustomAttributes(typeof(RequirePermissionAttribute), false)
                .Length > 0;

            // Assert
            hasPermissionAttribute.Should().BeTrue("DeleteProduct should require permission attribute");
        }

        #endregion

        #region HTTP Method Attribute Tests

        /// <summary>
        /// Test: Endpoints have correct HTTP method attributes
        /// Expected: GET, POST, DELETE methods defined
        /// </summary>
        [Fact]
        [Trait("Category", "HTTP")]
        public void Endpoints_HaveCorrectHttpMethods()
        {
            // Arrange & Act
            var adminMethod = _controllerType.GetMethod("AdminOnly");
            var managerMethod = _controllerType.GetMethod("ManagerAndAdmin");
            var getProductsMethod = _controllerType.GetMethod("GetProducts");
            var createMethod = _controllerType.GetMethod("CreateProduct");
            var deleteMethod = _controllerType.GetMethod("DeleteProduct");

            // Assert - GET methods
            ValidateHttpGetAttribute(adminMethod);
            ValidateHttpGetAttribute(managerMethod);
            ValidateHttpGetAttribute(getProductsMethod);

            // POST method
            ValidateHttpPostAttribute(createMethod);

            // DELETE method
            ValidateHttpDeleteAttribute(deleteMethod);
        }

        #endregion

        #region Helper Methods

        private static void ValidateHttpGetAttribute(MethodInfo? method)
        {
            method.Should().NotBeNull();
            var hasHttpGet = method?
                .GetCustomAttributes(typeof(HttpGetAttribute), false)
                .Length > 0;
            hasHttpGet.Should().BeTrue();
        }

        private static void ValidateHttpPostAttribute(MethodInfo? method)
        {
            method.Should().NotBeNull();
            var hasHttpPost = method?
                .GetCustomAttributes(typeof(HttpPostAttribute), false)
                .Length > 0;
            hasHttpPost.Should().BeTrue();
        }

        private static void ValidateHttpDeleteAttribute(MethodInfo? method)
        {
            method.Should().NotBeNull();
            var hasHttpDelete = method?
                .GetCustomAttributes(typeof(HttpDeleteAttribute), false)
                .Length > 0;
            hasHttpDelete.Should().BeTrue();
        }

        #endregion
    }
}
