using System.ComponentModel.DataAnnotations;

namespace KiriyamaServer.WebApi.UseCases.Auth;

/// <summary>登录请求：账号密码 + PKCE code_challenge。</summary>
public sealed class LoginRequest
{
    [Required]
    [EmailAddress]
    public required string Email { get; init; }

    [Required]
    public required string Password { get; init; }

    /// <summary>PKCE S256 的 code_challenge（客户端登录前生成）。</summary>
    [Required]
    public required string CodeChallenge { get; init; }
}
