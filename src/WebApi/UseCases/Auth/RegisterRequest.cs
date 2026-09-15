using System.ComponentModel.DataAnnotations;

namespace KiriyamaServer.WebApi.UseCases.Auth;

/// <summary>注册请求（camelCase 序列化）。</summary>
public sealed class RegisterRequest
{
    [Required]
    [EmailAddress]
    public required string Email { get; init; }

    [Required]
    [MinLength(8)]
    public required string Password { get; init; }

    /// <summary>显示名（可选，缺省用邮箱前缀）。</summary>
    public string? DisplayName { get; init; }
}
