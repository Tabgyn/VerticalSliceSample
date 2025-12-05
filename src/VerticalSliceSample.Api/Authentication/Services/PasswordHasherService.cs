using Microsoft.AspNetCore.Identity;

namespace VerticalSliceSample.Api.Authentication.Services;

/// <summary>
/// Interface for password hashing
/// </summary>
public interface IPasswordHasherService
{
    string HashPassword(string password);
    bool VerifyPassword(string hashedPassword, string providedPassword);
}

/// <summary>
/// Implementation using ASP.NET Core Identity PasswordHasher
/// </summary>
public class PasswordHasherService : IPasswordHasherService
{
    private readonly PasswordHasher<object> _passwordHasher;

    public PasswordHasherService()
    {
        _passwordHasher = new PasswordHasher<object>();
    }

    /// <summary>
    /// Creates password hash
    /// </summary>
    public string HashPassword(string password)
    {
        return _passwordHasher.HashPassword(null!, password);
    }

    /// <summary>
    /// Checks if the given password matches the hash
    /// </summary>
    /// <returns>true if the password is correct</returns>
    public bool VerifyPassword(string hashedPassword, string providedPassword)
    {
        var result = _passwordHasher.VerifyHashedPassword(
            null!,
            hashedPassword,
            providedPassword);

        return result == PasswordVerificationResult.Success ||
               result == PasswordVerificationResult.SuccessRehashNeeded;
    }
}
