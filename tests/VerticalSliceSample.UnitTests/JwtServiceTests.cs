using Microsoft.Extensions.Configuration;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using VerticalSliceSample.Api.Authentication.Services;
using VerticalSliceSample.Api.Database.Entities;

namespace VerticalSliceSample.UnitTests
{
    public class JwtServiceTests
    {
        [Fact]
        public void GenerateAccessToken_ShouldCreateValidToken()
        {
            // Arrange
            var configuration = new ConfigurationBuilder()
                .AddInMemoryCollection(new Dictionary<string, string?>
                {
                    ["Jwt:Key"] = "your-super-secret-key-with-at-least-32-characters",
                    ["Jwt:Issuer"] = "TestIssuer",
                    ["Jwt:Audience"] = "TestAudience",
                    ["Jwt:ExpirationInMinutes"] = "15"
                })
                .Build();

            var jwtService = new JwtService(configuration);

            var user = new User
            {
                Username = "test@email.com",
                Password = "hash",
                DisplayName = "Test User"
            };

            // Act
            var token = jwtService.GenerateAccessToken(user);

            // Assert
            Assert.NotNull(token);
            Assert.NotEmpty(token);

            var handler = new JwtSecurityTokenHandler();
            var jwtToken = handler.ReadJwtToken(token);

            Assert.Equal("TestIssuer", jwtToken.Issuer);
            Assert.Contains("TestAudience", jwtToken.Audiences);
            Assert.Contains(jwtToken.Claims, c => c.Type == ClaimTypes.NameIdentifier && c.Value == user.ReferenceId.ToString());
        }
    }
}
