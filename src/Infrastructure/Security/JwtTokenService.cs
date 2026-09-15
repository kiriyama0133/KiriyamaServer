using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Security.Cryptography;
using System.Text;
using KiriyamaServer.Application.Services;
using KiriyamaServer.Domain.Users;
using Microsoft.Extensions.Options;
using Microsoft.IdentityModel.Tokens;

namespace KiriyamaServer.Infrastructure.Security;

/// <summary>
/// JWT 令牌服务：签发访问令牌（JWT）、刷新令牌、以及 PKCE S256 摘要与授权码生成。
/// </summary>
public sealed class JwtTokenService(IOptions<JwtOptions> options) : ITokenService
{
    private readonly JwtOptions _options = options?.Value ?? throw new ArgumentNullException(nameof(options));

    public int AccessTokenLifetimeSeconds => _options.AccessTokenLifetimeSeconds;

    public int RefreshTokenLifetimeSeconds => _options.RefreshTokenLifetimeSeconds;

    public string IssueAccessToken(User user, ClientId clientId)
    {
        var now = DateTime.UtcNow;
        var credentials = new SigningCredentials(
            new SymmetricSecurityKey(Encoding.UTF8.GetBytes(_options.SigningKey)),
            SecurityAlgorithms.HmacSha256);

        var claims = new[]
        {
            new Claim(JwtRegisteredClaimNames.Sub, user.Id.ToString()),
            new Claim(JwtRegisteredClaimNames.Email, user.Email.Value),
            new Claim("displayName", user.DisplayName),
            new Claim("client_id", clientId.Value),
            new Claim(JwtRegisteredClaimNames.Jti, Guid.NewGuid().ToString()),
        };

        var token = new JwtSecurityToken(
            issuer: _options.Issuer,
            audience: _options.Audience,
            claims: claims,
            notBefore: now,
            expires: now.AddSeconds(_options.AccessTokenLifetimeSeconds),
            signingCredentials: credentials);

        return new JwtSecurityTokenHandler().WriteToken(token);
    }

    public (string Plain, string Hash) IssueRefreshToken()
    {
        byte[] bytes = RandomNumberGenerator.GetBytes(48);
        string plain = Convert.ToBase64String(bytes);
        return (plain, ComputeCodeChallenge(plain));
    }

    public string ComputeCodeChallenge(string codeVerifier)
    {
        byte[] hash = SHA256.HashData(Encoding.ASCII.GetBytes(codeVerifier));
        return Base64UrlEncode(hash);
    }

    public string GenerateAuthorizationCode()
    {
        byte[] bytes = RandomNumberGenerator.GetBytes(32);
        return Base64UrlEncode(bytes);
    }

    private static string Base64UrlEncode(byte[] data)
        => Convert.ToBase64String(data).TrimEnd('=').Replace('+', '-').Replace('/', '_');
}
