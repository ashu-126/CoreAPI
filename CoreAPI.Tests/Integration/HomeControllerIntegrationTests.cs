using FluentAssertions;
using System.Net;
using System.Net.Http.Headers;
using Xunit;

namespace CoreAPI.Tests.Integration
{
    /// <summary>
    /// Integration tests for HomeController endpoints
    /// Tests: Full HTTP request/response cycle with authorization
    /// </summary>
    public class HomeControllerIntegrationTests : IAsyncLifetime
    {
        private readonly HttpClient _httpClient;
        private readonly string _baseUrl = "http://localhost:5000";

        public HomeControllerIntegrationTests()
        {
            _httpClient = new HttpClient();
        }

        public Task InitializeAsync()
        {
            // Initialize test environment
            return Task.CompletedTask;
        }

        public Task DisposeAsync()
        {
            _httpClient?.Dispose();
            return Task.CompletedTask;
        }

        #region Admin Endpoint Integration Tests

        /// <summary>
        /// Test: Admin endpoint with valid authorization
        /// Note: Run with actual server running
        /// </summary>
        [Fact(Skip = "Integration test - requires running server")]
        [Trait("Category", "Integration")]
        [Trait("Endpoint", "AdminOnly")]
        public async Task AdminEndpoint_WithValidToken_ReturnsSuccessAsync()
        {
            // Arrange
            var token = "your-valid-jwt-token-here";
            _httpClient.DefaultRequestHeaders.Authorization = 
                new AuthenticationHeaderValue("Bearer", token);

            // Act
            var response = await _httpClient.GetAsync($"{_baseUrl}/api/home/admin");

            // Assert
            response.StatusCode.Should().Be(HttpStatusCode.OK);
        }

        /// <summary>
        /// Test: Admin endpoint without authorization
        /// Expected: 401 Unauthorized
        /// </summary>
        [Fact(Skip = "Integration test - requires running server")]
        [Trait("Category", "Integration")]
        [Trait("Endpoint", "AdminOnly")]
        public async Task AdminEndpoint_WithoutToken_ReturnsUnauthorizedAsync()
        {
            // Act
            var response = await _httpClient.GetAsync($"{_baseUrl}/api/home/admin");

            // Assert
            response.StatusCode.Should().Be(HttpStatusCode.Unauthorized);
        }

        #endregion

        #region Manager Endpoint Integration Tests

        /// <summary>
        /// Test: Manager endpoint with admin token
        /// Expected: 200 OK (admin role can access manager endpoint)
        /// </summary>
        [Fact(Skip = "Integration test - requires running server")]
        [Trait("Category", "Integration")]
        [Trait("Endpoint", "ManagerAndAdmin")]
        public async Task ManagerEndpoint_WithAdminToken_ReturnsSuccessAsync()
        {
            // Arrange
            var adminToken = "admin-jwt-token";
            _httpClient.DefaultRequestHeaders.Authorization = 
                new AuthenticationHeaderValue("Bearer", adminToken);

            // Act
            var response = await _httpClient.GetAsync($"{_baseUrl}/api/home/manager");

            // Assert
            response.StatusCode.Should().Be(HttpStatusCode.OK);
        }

        /// <summary>
        /// Test: Manager endpoint with manager token
        /// Expected: 200 OK
        /// </summary>
        [Fact(Skip = "Integration test - requires running server")]
        [Trait("Category", "Integration")]
        [Trait("Endpoint", "ManagerAndAdmin")]
        public async Task ManagerEndpoint_WithManagerToken_ReturnsSuccessAsync()
        {
            // Arrange
            var managerToken = "manager-jwt-token";
            _httpClient.DefaultRequestHeaders.Authorization = 
                new AuthenticationHeaderValue("Bearer", managerToken);

            // Act
            var response = await _httpClient.GetAsync($"{_baseUrl}/api/home/manager");

            // Assert
            response.StatusCode.Should().Be(HttpStatusCode.OK);
        }

        #endregion

        #region Product Endpoints Integration Tests

        /// <summary>
        /// Test: Get Products endpoint
        /// Expected: 200 OK with valid data
        /// </summary>
        [Fact(Skip = "Integration test - requires running server")]
        [Trait("Category", "Integration")]
        [Trait("Endpoint", "GetProducts")]
        public async Task GetProductsEndpoint_WithValidToken_ReturnsSuccessAsync()
        {
            // Arrange
            var token = "valid-jwt-token";
            _httpClient.DefaultRequestHeaders.Authorization = 
                new AuthenticationHeaderValue("Bearer", token);

            // Act
            var response = await _httpClient.GetAsync($"{_baseUrl}/api/home/products");

            // Assert
            response.StatusCode.Should().Be(HttpStatusCode.OK);
            var content = await response.Content.ReadAsStringAsync();
            content.Should().Contain("Products list");
        }

        /// <summary>
        /// Test: Create Product endpoint
        /// Expected: 200 OK
        /// </summary>
        [Fact(Skip = "Integration test - requires running server")]
        [Trait("Category", "Integration")]
        [Trait("Endpoint", "CreateProduct")]
        public async Task CreateProductEndpoint_WithValidToken_ReturnsSuccessAsync()
        {
            // Arrange
            var token = "valid-jwt-token";
            _httpClient.DefaultRequestHeaders.Authorization = 
                new AuthenticationHeaderValue("Bearer", token);

            var content = new StringContent(
                "{\"name\":\"Test Product\",\"price\":99.99}",
                System.Text.Encoding.UTF8,
                "application/json");

            // Act
            var response = await _httpClient.PostAsync($"{_baseUrl}/api/home/products", content);

            // Assert
            response.StatusCode.Should().Be(HttpStatusCode.OK);
        }

        /// <summary>
        /// Test: Delete Product endpoint
        /// Expected: 200 OK
        /// </summary>
        [Theory(Skip = "Integration test - requires running server")]
        [InlineData(1)]
        [InlineData(100)]
        [Trait("Category", "Integration")]
        [Trait("Endpoint", "DeleteProduct")]
        public async Task DeleteProductEndpoint_WithValidId_ReturnsSuccessAsync(int productId)
        {
            // Arrange
            var token = "valid-jwt-token";
            _httpClient.DefaultRequestHeaders.Authorization = 
                new AuthenticationHeaderValue("Bearer", token);

            // Act
            var response = await _httpClient.DeleteAsync($"{_baseUrl}/api/home/products/{productId}");

            // Assert
            response.StatusCode.Should().Be(HttpStatusCode.OK);
        }

        #endregion

        #region Authorization Tests

        /// <summary>
        /// Test: Endpoints without token return 401
        /// Expected: Unauthorized
        /// </summary>
        [Theory(Skip = "Integration test - requires running server")]
        [InlineData("/api/home/admin")]
        [InlineData("/api/home/manager")]
        [InlineData("/api/home/products")]
        [Trait("Category", "Integration")]
        [Trait("Security", "Authorization")]
        public async Task ProtectedEndpoints_WithoutToken_ReturnUnauthorizedAsync(string endpoint)
        {
            // Act
            var response = await _httpClient.GetAsync($"{_baseUrl}{endpoint}");

            // Assert
            response.StatusCode.Should().Be(HttpStatusCode.Unauthorized);
        }

        /// <summary>
        /// Test: Invalid token returns 401
        /// Expected: Unauthorized
        /// </summary>
        [Fact(Skip = "Integration test - requires running server")]
        [Trait("Category", "Integration")]
        [Trait("Security", "Authorization")]
        public async Task Endpoints_WithInvalidToken_ReturnUnauthorizedAsync()
        {
            // Arrange
            _httpClient.DefaultRequestHeaders.Authorization = 
                new AuthenticationHeaderValue("Bearer", "invalid-token");

            // Act
            var response = await _httpClient.GetAsync($"{_baseUrl}/api/home/admin");

            // Assert
            response.StatusCode.Should().Be(HttpStatusCode.Unauthorized);
        }

        #endregion
    }
}
