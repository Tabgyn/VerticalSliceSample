using Microsoft.Extensions.DependencyInjection;
using System.Text.Json;
using VerticalSliceSample.Api.Authentication.Services;
using VerticalSliceSample.Api.Database;
using VerticalSliceSample.Api.Database.Entities;

namespace VerticalSliceSample.IntegrationTests.Authentication
{
    [CollectionDefinition(nameof(IntegrationTestsCollection))]
    public class IntegrationTestsCollection : ICollectionFixture<PostgreSqlContainerFixture> { }

    [Collection(nameof(IntegrationTestsCollection))]
    public class AuthenticationTests : IAsyncLifetime
    {
        private readonly CustomWebApplicationFactory _factory;
        private readonly HttpClient _client;
        private readonly ApplicationDbContext _dbContext;

        public AuthenticationTests(PostgreSqlContainerFixture fixture)
        {
            _factory = new CustomWebApplicationFactory(fixture.Postgres.GetConnectionString());
            _client = _factory.CreateClient();

            // Create a scope to retrieve a scoped instance of the DB context.
            // This allows direct interaction with the database for setup and teardown.
            var scope = _factory.Services.CreateScope();
            _dbContext = scope.ServiceProvider.GetRequiredService<ApplicationDbContext>();
        }

        public async Task InitializeAsync()
        {
            // Clean the database to ensure test isolation.
            _dbContext.Users.RemoveRange(_dbContext.Users);
            await _dbContext.SaveChangesAsync();
        }

        // Dispose resources to maintain isolation after each test run.
        public Task DisposeAsync()
        {
            _client.Dispose();
            _factory.Dispose();
            _dbContext.Dispose();
            return Task.CompletedTask;
        }

        [Fact]
        public async Task Login_ShouldBeSuccess()
        {
            // Arrange
            var hasher = new PasswordHasherService();
            var existingUser = new User
            {
                Username = "john.doe",
                Password = hasher.HashPassword("pass@123"),
                DisplayName = "John Doe",
            };
            _dbContext.Users.Add(existingUser);
            await _dbContext.SaveChangesAsync();

            var httpContent = new StringContent(JsonSerializer.Serialize(new
            {
                Username = "john.doe",
                Password = "pass@123",
            }), System.Text.Encoding.UTF8, "application/json");

            // Act
            var response = await _client.PostAsync("/auth/login", httpContent);

            // Assert
            response.EnsureSuccessStatusCode();
            Assert.Equal("application/json; charset=utf-8", response?.Content.Headers.ContentType?.ToString());
        }

        [Fact]
        public async Task Signup_ShouldBeSuccess()
        {
            // Arrange
            var httpContent = new StringContent(JsonSerializer.Serialize(new
            {
                Username = "john.doe",
                Password = "pass@123",
                Name = "John Doe"
            }), System.Text.Encoding.UTF8, "application/json");

            // Act
            var response = await _client.PostAsync("/auth/signup", httpContent);

            // Assert
            response.EnsureSuccessStatusCode();
            Assert.Equal("application/json; charset=utf-8", response?.Content.Headers.ContentType?.ToString());
        }
    }
}