using VerticalSliceSample.Api.Authentication.Services;

namespace VerticalSliceSample.UnitTests
{
    public class PasswordHasherServiceTests
    {
        [Fact]
        public void HashPassword_ShouldReturnDifferentHashesForSamePassword()
        {
            // Arrange
            var hasher = new PasswordHasherService();
            var password = "Password@123";

            // Act
            var hash1 = hasher.HashPassword(password);
            var hash2 = hasher.HashPassword(password);

            // Assert
            Assert.NotEqual(hash1, hash2);
        }

        [Fact]
        public void VerifyPassword_WithCorrectPassword_ShouldReturnTrue()
        {
            // Arrange
            var hasher = new PasswordHasherService();
            var password = "Password@123";
            var hash = hasher.HashPassword(password);

            // Act
            var result = hasher.VerifyPassword(hash, password);

            // Assert
            Assert.True(result);
        }

        [Fact]
        public void VerifyPassword_WithIncorrectPassword_ShouldReturnFalse()
        {
            // Arrange
            var hasher = new PasswordHasherService();
            var password = "Password@123";
            var hash = hasher.HashPassword(password);

            // Act
            var result = hasher.VerifyPassword(hash, "OtherPassword@123");

            // Assert
            Assert.False(result);
        }

        [Fact]
        public void VerifyPassword_WithEmptyPassword_ShouldReturnFalse()
        {
            // Arrange
            var hasher = new PasswordHasherService();
            var hash = hasher.HashPassword("Password@123");

            // Act
            var result = hasher.VerifyPassword(hash, string.Empty);

            // Assert
            Assert.False(result);
        }
    }
}
