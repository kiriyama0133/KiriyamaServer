namespace KiriyamaServer.Infrastructure.Security;

/// <summary>JWT 配置项（对应 appsettings 的 Jwt 节）。</summary>
public sealed class JwtOptions
{
    public const string Position = "Jwt";

    public string Issuer { get; set; } = "KiriyamaServer";
    public string Audience { get; set; } = "kiriyamalauncher";
    public string SigningKey { get; set; } = "change-me-in-production-32-chars-min";
    public int AccessTokenLifetimeSeconds { get; set; } = 3600;

    /// <summary>刷新令牌有效期（秒），默认 7 天。</summary>
    public int RefreshTokenLifetimeSeconds { get; set; } = 7 * 24 * 60 * 60;
}
