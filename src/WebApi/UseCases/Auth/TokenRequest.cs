using System.ComponentModel.DataAnnotations;
using Microsoft.AspNetCore.Mvc;

namespace KiriyamaServer.WebApi.UseCases.Auth;

/// <summary>
/// OAuth token 端点请求（application/x-www-form-urlencoded）。
/// grant_type: authorization_code | refresh_token
/// 注意：form 字段名是 OAuth 标准的 snake_case（grant_type / code_verifier / client_id），
/// 与 C# 属性名不同，必须用 [FromForm(Name = ...)] 显式指定，否则 [FromForm] 按属性名匹配会绑定失败。
/// </summary>
public sealed class TokenRequest
{
    [FromForm(Name = "grant_type")]
    [Required]
    public required string GrantType { get; init; }

    /// <summary>authorization_code grant 时必填。</summary>
    [FromForm(Name = "code")]
    public string? Code { get; init; }

    /// <summary>authorization_code grant 时必填（PKCE code_verifier）。</summary>
    [FromForm(Name = "code_verifier")]
    public string? CodeVerifier { get; init; }

    /// <summary>refresh_token grant 时必填。</summary>
    [FromForm(Name = "refresh_token")]
    public string? RefreshToken { get; init; }

    [FromForm(Name = "client_id")]
    [Required]
    public required string ClientId { get; init; }
}
