using Microsoft.AspNetCore.Mvc.Testing;

namespace VerticalSliceSample.IntegrationTests.Authentication
{
    public class AuthenticationTests(WebApplicationFactory<Program> factory) : IClassFixture<WebApplicationFactory<Program>>
    {
        private readonly WebApplicationFactory<Program> _factory = factory;

        [Theory]
        [InlineData("/auth/login")]
        [InlineData("/auth/signup")]
        public async Task Get_EndpointsReturnSuccessAndCorrectContentType(string url)
        {
            // Arrange
            var client = _factory.CreateClient();

            // Act
            var response = await client.GetAsync(url);

            // Assert
            response.EnsureSuccessStatusCode();
            Assert.Equal("text/html; charset=utf-8", response?.Content.Headers.ContentType?.ToString());
        }
    }
}