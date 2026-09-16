using KiriyamaServer.Application.Interfaces;

namespace KiriyamaServer.Application.Boundaries.Auth;

/// <summary>登录用例的输出：一次授权码（供 PKCE 换 token）+ 显示名。</summary>
public sealed class LoginOutput : IOutputType
{
    /// <summary>授权码（短时有效，一次性）。</summary>
    public string Code { get; }
    public DateTime ExpiresAt { get; }

    /// <summary>用户显示名（供客户端登录后立即显示名称，而非邮箱）。</summary>
    public string DisplayName { get; }

    public LoginOutput(string code, DateTime expiresAt, string displayName)
    {
        Code = code;
        ExpiresAt = expiresAt;
        DisplayName = displayName;
    }
}

/// <summary>登录用例的输出端口。</summary>
public interface ILoginOutputPort : IErrorHandler
{
    void Standard(LoginOutput output);
}
