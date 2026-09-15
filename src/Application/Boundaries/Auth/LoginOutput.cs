using KiriyamaServer.Application.Interfaces;

namespace KiriyamaServer.Application.Boundaries.Auth;

/// <summary>登录用例的输出：一次授权码（供 PKCE 换 token）。</summary>
public sealed class LoginOutput : IOutputType
{
    /// <summary>授权码（短时有效，一次性）。</summary>
    public string Code { get; }
    public DateTime ExpiresAt { get; }

    public LoginOutput(string code, DateTime expiresAt)
    {
        Code = code;
        ExpiresAt = expiresAt;
    }
}

/// <summary>登录用例的输出端口。</summary>
public interface ILoginOutputPort : IErrorHandler
{
    void Standard(LoginOutput output);
}
