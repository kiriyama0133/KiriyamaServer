using KiriyamaServer.Application.Interfaces;

namespace KiriyamaServer.Application.Boundaries.Auth;

/// <summary>令牌兑换用例的输出：OAuth 标准 token 响应。</summary>
public sealed class TokenOutput : IOutputType
{
    public string AccessToken { get; }
    public string TokenType { get; }
    public int ExpiresIn { get; }
    public string RefreshToken { get; }

    public TokenOutput(string accessToken, string tokenType, int expiresIn, string refreshToken)
    {
        AccessToken = accessToken;
        TokenType = tokenType;
        ExpiresIn = expiresIn;
        RefreshToken = refreshToken;
    }
}

/// <summary>令牌兑换用例的输出端口。</summary>
public interface ITokenOutputPort : IErrorHandler
{
    void Standard(TokenOutput output);
}
