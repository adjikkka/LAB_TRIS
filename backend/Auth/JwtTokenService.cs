using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;
using Microsoft.IdentityModel.Tokens;
using backend.Models;

namespace backend.Auth;

// Учебный JWT (ЛР11). Способ входа выбран один — токен в заголовке
// Authorization, cookie-сессии не смешиваются.
public static class AuthOptions
{
    public const string Issuer = "dorm-backend";
    public const string Audience = "dorm-frontend";

    // Ключ подписи берётся из конфигурации (user-secrets).
    // Значение по умолчанию — только для учебного запуска в Development.
    public const string DevFallbackKey = "dev-only-signing-key-for-lab-11-0123456789abcdef";

    public static string GetSigningKey(IConfiguration configuration)
    {
        var key = configuration["Jwt:Key"];
        return string.IsNullOrWhiteSpace(key) ? DevFallbackKey : key;
    }
}

public sealed class JwtTokenService
{
    private readonly IConfiguration _configuration;

    public JwtTokenService(IConfiguration configuration)
    {
        _configuration = configuration;
    }

    public (string Token, DateTimeOffset ExpiresAt) CreateToken(User user)
    {
        var claims = new List<Claim>
        {
            new(ClaimTypes.NameIdentifier, user.Id.ToString()),
            new(ClaimTypes.Name, user.Name),
            new(ClaimTypes.Role, user.Role)
        };

        var key = new SymmetricSecurityKey(
            Encoding.UTF8.GetBytes(AuthOptions.GetSigningKey(_configuration)));
        var credentials = new SigningCredentials(key, SecurityAlgorithms.HmacSha256);

        var expires = DateTimeOffset.UtcNow.AddHours(8);
        var token = new JwtSecurityToken(
            issuer: AuthOptions.Issuer,
            audience: AuthOptions.Audience,
            claims: claims,
            notBefore: DateTime.UtcNow,
            expires: expires.UtcDateTime,
            signingCredentials: credentials);

        return (new JwtSecurityTokenHandler().WriteToken(token), expires);
    }
}

// Данные вошедшего пользователя достаются из claims токена.
public static class ClaimsPrincipalExtensions
{
    public static int GetUserId(this ClaimsPrincipal principal) =>
        int.TryParse(principal.FindFirstValue(ClaimTypes.NameIdentifier), out var id) ? id : 0;

    public static string GetUserRole(this ClaimsPrincipal principal) =>
        principal.FindFirstValue(ClaimTypes.Role) ?? string.Empty;
}
