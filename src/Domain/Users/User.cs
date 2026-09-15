namespace KiriyamaServer.Domain.Users;

/// <summary>
/// 用户（聚合根）。代表一个可注册、登录、通过 PKCE+OAuth 换取访问令牌的账户。
/// </summary>
public class User
{
    private readonly List<RefreshToken> _refreshTokens = new();
    private readonly List<AuthorizationCode> _authorizationCodes = new();

    public Guid Id { get; protected set; }

    /// <summary>邮箱（唯一标识，登录凭据）。</summary>
    public Email Email { get; protected set; }

    /// <summary>密码哈希（由应用层 IPasswordHasher 生成，绝不存明文）。</summary>
    public string PasswordHash { get; protected set; }

    /// <summary>显示名。</summary>
    public string DisplayName { get; protected set; }

    /// <summary>创建时间。</summary>
    public DateTime CreatedAt { get; protected set; }

    /// <summary>刷新令牌集合（只读视图）。</summary>
    public IReadOnlyList<RefreshToken> RefreshTokens => _refreshTokens;

    /// <summary>已签发的授权码集合（只读视图）。</summary>
    public IReadOnlyList<AuthorizationCode> AuthorizationCodes => _authorizationCodes;

    protected User()
    {
    }

    public User(Email email, string passwordHash, string displayName)
    {
        if (string.IsNullOrWhiteSpace(passwordHash))
        {
            throw new DomainException("Password hash cannot be empty.");
        }

        Id = Guid.NewGuid();
        Email = email ?? throw new DomainException("Email cannot be null.");
        PasswordHash = passwordHash;
        DisplayName = string.IsNullOrWhiteSpace(displayName) ? email.Value : displayName.Trim();
        CreatedAt = DateTime.UtcNow;
    }

    /// <summary>发放一个新的刷新令牌（存 hash，明文由调用方自行返回）。</summary>
    public RefreshToken IssueRefreshToken(string tokenHash, DateTime expiresAt)
    {
        var token = new RefreshToken(tokenHash, DateTime.UtcNow, expiresAt);
        _refreshTokens.Add(token);
        return token;
    }

    /// <summary>撤销某个刷新令牌。</summary>
    public bool RevokeRefreshToken(string tokenHash)
    {
        RefreshToken? token = _refreshTokens.FirstOrDefault(t => t.TokenHash == tokenHash);
        if (token is null)
        {
            return false;
        }

        token.Revoke();
        return true;
    }

    /// <summary>查找仍有效（未撤销、未过期）的刷新令牌。</summary>
    public RefreshToken? FindActiveRefreshToken(string tokenHash, DateTime now)
        => _refreshTokens.FirstOrDefault(t => t.TokenHash == tokenHash && t.IsActive(now));

    /// <summary>签发一个一次性授权码（记录其 PKCE code_challenge）。</summary>
    public AuthorizationCode IssueAuthorizationCode(string code, string codeChallenge, DateTime expiresAt)
    {
        var authorizationCode = new AuthorizationCode(code, codeChallenge, DateTime.UtcNow, expiresAt);
        _authorizationCodes.Add(authorizationCode);
        return authorizationCode;
    }

    /// <summary>查找并消费一个授权码（校验 code_verifier 对应的 challenge）。</summary>
    public AuthorizationCode? ConsumeAuthorizationCode(string code, string codeChallenge, DateTime now)
    {
        AuthorizationCode? authorizationCode = _authorizationCodes.FirstOrDefault(c => c.Code == code);
        return authorizationCode is not null && authorizationCode.TryConsume(codeChallenge, now)
            ? authorizationCode
            : null;
    }
}
