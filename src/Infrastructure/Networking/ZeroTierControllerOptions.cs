namespace KiriyamaServer.Infrastructure.Networking;

/// <summary>
/// ZeroTier Controller 的运行时配置。
/// </summary>
public sealed class ZeroTierControllerOptions
{
    /// <summary>
    /// Controller 后端类型：<c>SelfHosted</c>（本机 zerotier-one 控制端口）或
    /// <c>Official</c>（my.zerotier.com REST API）。
    /// </summary>
    public string Backend { get; set; } = "SelfHosted";

    /// <summary>
    /// 自建 Controller 控制端口的地址。留空时由平台适配器决定默认值
    /// （<c>http://localhost:9993</c>）。
    /// </summary>
    public string? ControlUrl { get; set; }

    /// <summary>
    /// 自建 Controller 的认证令牌（对应 zerotier-one 的 authtoken.secret）。
    /// 留空时由平台适配器从默认路径读取（Linux：<c>/var/lib/zerotier-one/authtoken.secret</c>，
    /// Windows：<c>C:\ProgramData\ZeroTier\One\authtoken.secret</c>）。
    /// </summary>
    public string? AuthToken { get; set; }

    /// <summary>要管理的 ZeroTier 网络 ID（16 位十六进制）。</summary>
    public string? NetworkId { get; set; }

    /// <summary>房间隔离所用的 Tag 定义名（默认 room_id）。</summary>
    public string RoomTagName { get; set; } = "room_id";

    /// <summary>官方 API 的访问令牌（Backend=Official 时使用）。</summary>
    public string? ApiToken { get; set; }

    /// <summary>官方 API 地址（Backend=Official 时使用）。</summary>
    public string ApiBaseUrl { get; set; } = "https://api.zerotier.com";
}
