using KiriyamaServer.Application.Exceptions;
using KiriyamaServer.Application.Interfaces;
using KiriyamaServer.Domain.Users;

namespace KiriyamaServer.Application.Boundaries.Auth;

/// <summary>
/// 令牌兑换用例的输入（PKCE + OAuth token endpoint）。
/// </summary>
public sealed class TokenInput : IInputType
{
    public string GrantType { get; }
    public string Code { get; }
    public string CodeVerifier { get; }
    public string? RefreshToken { get; }
    public ClientId ClientId { get; }

    public TokenInput(string grantType, string code, string codeVerifier, string? refreshToken, string clientId)
    {
        if (string.IsNullOrWhiteSpace(grantType))
        {
            throw new InputValidationException("Grant type cannot be empty.");
        }

        GrantType = grantType;
        Code = code;
        CodeVerifier = codeVerifier;
        RefreshToken = refreshToken;
        ClientId = new ClientId(clientId);
    }
}
