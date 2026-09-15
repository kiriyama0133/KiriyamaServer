using KiriyamaServer.Application.Exceptions;
using KiriyamaServer.Application.Interfaces;
using KiriyamaServer.Domain.Users;

namespace KiriyamaServer.Application.Boundaries.Auth;

/// <summary>登录（密码校验，签发 PKCE 授权码）用例的输入。</summary>
public sealed class LoginInput : IInputType
{
    public Email Email { get; }
    public string Password { get; }

    /// <summary>PKCE code_challenge（客户端登录前生成的 S256 摘要）。</summary>
    public string CodeChallenge { get; }

    public LoginInput(string email, string password, string codeChallenge)
    {
        if (string.IsNullOrWhiteSpace(password))
        {
            throw new InputValidationException("Password cannot be empty.");
        }

        if (string.IsNullOrWhiteSpace(codeChallenge))
        {
            throw new InputValidationException("Code challenge cannot be empty.");
        }

        Email = new Email(email);
        Password = password;
        CodeChallenge = codeChallenge;
    }
}
