namespace KiriyamaServer.Domain.Users;

/// <summary>
/// 刷新令牌（值对象）。用于延长会话、换取新的访问令牌。
/// </summary>
public sealed class RefreshToken
{
    /// <summary>令牌字符串（服务端存 hash，明文只返回一次给客户端）。</summary>
    public string TokenHash { get; protected set; }

    public DateTime CreatedAt { get; protected set; }

    public DateTime ExpiresAt { get; protected set; }

    /// <summary>是否已被撤销（用于登出/轮换时失效旧令牌）。</summary>
    public bool IsRevoked { get; protected set; }

    protected RefreshToken()
    {
    }

    public RefreshToken(string tokenHash, DateTime createdAt, DateTime expiresAt)
    {
        if (string.IsNullOrWhiteSpace(tokenHash))
        {
            throw new DomainException("Refresh token cannot be empty.");
        }

        if (expiresAt <= createdAt)
        {
            throw new DomainException("Refresh token expiry must be after creation.");
        }

        TokenHash = tokenHash;
        CreatedAt = createdAt;
        ExpiresAt = expiresAt;
    }

    public bool IsExpired(DateTime now) => now >= ExpiresAt;

    public bool IsActive(DateTime now) => !IsRevoked && !IsExpired(now);

    public void Revoke() => IsRevoked = true;
}
