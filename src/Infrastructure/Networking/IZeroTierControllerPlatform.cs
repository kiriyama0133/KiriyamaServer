namespace KiriyamaServer.Infrastructure.Networking;

/// <summary>
/// ZeroTier Controller 的平台适配（驱动端口）。
///
/// 自建 Controller 的认证令牌（authtoken.secret）与默认控制端口随操作系统不同：
///   - Linux（Debian/Ubuntu 等）：<c>/var/lib/zerotier-one/authtoken.secret</c>
///   - Windows：<c>C:\ProgramData\ZeroTier\One\authtoken.secret</c>
///
/// 通过本接口把「定位认证令牌与控制端口」的平台差异隔离出去，
/// 让 <see cref="SelfHostedZeroTierController"/> 只关心 HTTP 契约，不关心部署平台。
/// </summary>
public interface IZeroTierControllerPlatform
{
    /// <summary>平台标识（如 "linux" / "windows"），用于日志与诊断。</summary>
    string PlatformName { get; }

    /// <summary>默认的控制端口 URL（未显式配置 ControlUrl 时使用）。</summary>
    string DefaultControlUrl { get; }

    /// <summary>
    /// 解析认证令牌。优先使用显式配置的 AuthToken；未配置时从本平台的
    /// authtoken.secret 默认路径读取。
    /// </summary>
    Task<string> ResolveAuthTokenAsync(string? configuredToken, CancellationToken cancellationToken = default);
}
