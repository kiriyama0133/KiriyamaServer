using KiriyamaServer.Domain.Users;

namespace KiriyamaServer.Application.Services;

/// <summary>
/// 令牌服务（驱动端口）。签发/校验访问令牌（JWT）与刷新令牌，以及 PKCE 摘要。
/// </summary>
public interface ITokenService
{
    /// <summary>访问令牌有效期（秒）。</summary>
    int AccessTokenLifetimeSeconds { get; }

    /// <summary>刷新令牌有效期（秒）。</summary>
    int RefreshTokenLifetimeSeconds { get; }

    /// <summary>为指定用户签发访问令牌（JWT）。</summary>
    string IssueAccessToken(User user, ClientId clientId);

    /// <summary>签发一个刷新令牌，返回（明文, hash）二元组。明文仅返回客户端一次。</summary>
    (string Plain, string Hash) IssueRefreshToken();

    /// <summary>计算 PKCE S256 的 code_challenge（对 code_verifier 做 SHA-256 + base64url）。</summary>
    string ComputeCodeChallenge(string codeVerifier);

    /// <summary>生成一个授权码。</summary>
    string GenerateAuthorizationCode();
}
