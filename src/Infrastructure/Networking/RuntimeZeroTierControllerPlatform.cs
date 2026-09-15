using Microsoft.Extensions.Logging;

namespace KiriyamaServer.Infrastructure.Networking;

/// <summary>
/// 基于运行时操作系统判断的 ZeroTier Controller 平台适配。
///
/// 在进程运行时用 <see cref="OperatingSystem"/> 判定当前平台，
/// 定位 authtoken.secret 的默认路径与默认控制端口：
///   - Windows：<c>C:\ProgramData\ZeroTier\One\authtoken.secret</c>
///   - Linux：<c>/var/lib/zerotier-one/authtoken.secret</c>
/// </summary>
public sealed class RuntimeZeroTierControllerPlatform(ILogger<RuntimeZeroTierControllerPlatform> logger) : IZeroTierControllerPlatform
{
    private readonly ILogger<RuntimeZeroTierControllerPlatform> _logger = logger ?? throw new ArgumentNullException(nameof(logger));

    /// <inheritdoc />
    public string PlatformName => OperatingSystem.IsWindows() ? "windows" : "linux";

    /// <inheritdoc />
    public string DefaultControlUrl => "http://localhost:9993";

    /// <inheritdoc />
    public async Task<string> ResolveAuthTokenAsync(string? configuredToken, CancellationToken cancellationToken = default)
    {
        // 优先使用显式配置的令牌。
        if (!string.IsNullOrWhiteSpace(configuredToken))
        {
            return configuredToken.Trim();
        }

        string path = OperatingSystem.IsWindows()
            ? Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.CommonApplicationData), "ZeroTier", "One", "authtoken.secret")
            : "/var/lib/zerotier-one/authtoken.secret";

        if (!File.Exists(path))
        {
            throw new FileNotFoundException($"未找到 ZeroTier authtoken.secret（平台 {PlatformName}，路径 {path}）。请在 ZeroTier:AuthToken 配置中显式提供。", path);
        }

        string token = (await File.ReadAllTextAsync(path, cancellationToken).ConfigureAwait(false)).Trim();
        if (string.IsNullOrWhiteSpace(token))
        {
            throw new InvalidOperationException($"authtoken.secret 内容为空（路径 {path}）。");
        }

        _logger.LogDebug("已从平台默认路径读取 ZeroTier authtoken：{Path}", path);
        return token;
    }
}
