namespace KiriyamaServer.Domain.Users;

/// <summary>
/// PKCE 授权码（实体）。授权码模式下，服务端签发一次性授权码，
/// 记录其 PKCE code_challenge，供 token 端点校验 code_verifier。
/// </summary>
public sealed class AuthorizationCode
{
    /// <summary>授权码字符串（客户端凭此换 token）。</summary>
    public string Code { get; protected set; }

    /// <summary>PKCE 的 code_challenge（S256 的 base64url 摘要）。</summary>
    public string CodeChallenge { get; protected set; }

    public DateTime CreatedAt { get; protected set; }

    public DateTime ExpiresAt { get; protected set; }

    /// <summary>是否已被使用（一次性授权码，换 token 后失效）。</summary>
    public bool IsUsed { get; protected set; }

    protected AuthorizationCode()
    {
    }

    public AuthorizationCode(string code, string codeChallenge, DateTime createdAt, DateTime expiresAt)
    {
        if (string.IsNullOrWhiteSpace(code))
        {
            throw new DomainException("Authorization code cannot be empty.");
        }

        if (string.IsNullOrWhiteSpace(codeChallenge))
        {
            throw new DomainException("Code challenge cannot be empty.");
        }

        if (expiresAt <= createdAt)
        {
            throw new DomainException("Authorization code expiry must be after creation.");
        }

        Code = code;
        CodeChallenge = codeChallenge;
        CreatedAt = createdAt;
        ExpiresAt = expiresAt;
    }

    public bool IsExpired(DateTime now) => now >= ExpiresAt;

    /// <summary>校验 code_verifier（由应用层计算 S256 后比较），匹配则标记已使用。</summary>
    public bool TryConsume(string codeChallenge, DateTime now)
    {
        if (IsUsed || IsExpired(now) || CodeChallenge != codeChallenge)
        {
            return false;
        }

        IsUsed = true;
        return true;
    }
}
