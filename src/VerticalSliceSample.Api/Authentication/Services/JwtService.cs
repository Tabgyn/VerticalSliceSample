using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;
using Microsoft.IdentityModel.Tokens;
using VerticalSliceSample.Api.Database.Entities;

namespace VerticalSliceSample.Api.Authentication.Services;

public interface IJwtService
{
    string GenerateAccessToken(User user);
}

public class JwtService(IConfiguration configuration) : IJwtService
{
    private readonly IConfiguration _configuration = configuration;
    private readonly JwtSecurityTokenHandler _tokenHandler = new();

    /// <summary>
    /// Generates a JWT Access Token for the user
    /// </summary>
    public string GenerateAccessToken(User user)
    {
        var key = new SymmetricSecurityKey(
            Encoding.UTF8.GetBytes(_configuration["Jwt:Key"]!));
        var credentials = new SigningCredentials(key, SecurityAlgorithms.HmacSha256);
        var expirationMinutes = int.Parse(
            _configuration["Jwt:ExpirationInMinutes"] ?? "15");
        var token = new JwtSecurityToken(
            issuer: _configuration["Jwt:Issuer"],
            audience: _configuration["Jwt:Audience"],
            claims: [new(ClaimTypes.NameIdentifier, user.ReferenceId.ToString())],
            expires: DateTime.UtcNow.AddMinutes(expirationMinutes),
            signingCredentials: credentials
        );

        return _tokenHandler.WriteToken(token);
    }
}
