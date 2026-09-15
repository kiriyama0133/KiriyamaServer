using System.Net;
using System.Text;
using System.Text.Json;
using KiriyamaServer.Application.Services;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;

namespace KiriyamaServer.Infrastructure.Networking;

/// <summary>
/// 自建 ZeroTier Controller 实现：直接调用本机 zerotier-one 控制端口（默认 9993）
/// 的 <c>/controller</c> 接口，为网络下发 Tag 定义与 Flow Rules（<c>rulesSource</c>），
/// 并给成员节点打/清 Tag。
///
/// 平台差异（authtoken 路径、控制端口）交由 <see cref="IZeroTierControllerPlatform"/> 抽象。
///
/// 认证：HTTP 头 <c>X-ZT1-Auth</c> 携带 authtoken.secret 内容。
/// 契约要点（自建 Controller 的 /controller API）：
///   - 网络配置 <c>POST /controller/network/{networkId}</c>：
///     body 的 <c>rulesSource</c> 是 Flow Rules DSL 文本（tag 定义也写在 DSL 里）。
///   - 成员标签 <c>POST /controller/network/{networkId}/member/{nodeId}</c>：
///     body 的 <c>tags</c> 是 [tagId, tagValue] 二维数组。
/// </summary>
public sealed class SelfHostedZeroTierController(
    IOptions<ZeroTierControllerOptions> options,
    IHttpClientFactory httpClientFactory,
    IZeroTierControllerPlatform platform,
    ILogger<SelfHostedZeroTierController> logger) : IZeroTierController
{
    // 与 rulesSource DSL 里 `tag {name} id {RoomTagId}` 保持一致。
    private const int RoomTagId = 1;

    private static readonly JsonSerializerOptions JsonOptions = new() { PropertyNamingPolicy = JsonNamingPolicy.CamelCase };

    private readonly ZeroTierControllerOptions _options = options?.Value ?? throw new ArgumentNullException(nameof(options));
    private readonly IHttpClientFactory _httpClientFactory = httpClientFactory ?? throw new ArgumentNullException(nameof(httpClientFactory));
    private readonly IZeroTierControllerPlatform _platform = platform ?? throw new ArgumentNullException(nameof(platform));
    private readonly ILogger<SelfHostedZeroTierController> _logger = logger ?? throw new ArgumentNullException(nameof(logger));

    private string NetworkId => _options.NetworkId
        ?? throw new InvalidOperationException("ZeroTier:NetworkId 未配置。");

    /// <inheritdoc />
    public async Task EnsureNetworkAsync(CancellationToken cancellationToken = default)
    {
        string networkId = NetworkId;
        string url = $"{ControlUrl}/controller/network/{networkId}";

        // 以 rulesSource DSL 下发 Tag 定义 + 房间隔离 Flow Rules。
        // 语义（客户端隔离模式）：
        //   1) 先放行 ARP，保证节点互相发现；
        //   2) 声明 room_id 标签（id=RoomTagId，默认 0）；
        //   3) 若双方 room_id 不都等于 1（即不在同一房间），break → 交由末尾默认 drop。
        //   4) 其余（同房间）放行。
        var body = new NetworkConfig
        {
            RulesSource = BuildRulesSource(),
        };

        string authToken = await _platform.ResolveAuthTokenAsync(_options.AuthToken, cancellationToken).ConfigureAwait(false);
        await PostAsync(url, body, authToken, cancellationToken).ConfigureAwait(false);
        _logger.LogInformation("ZeroTier 网络 {NetworkId} 已下发房间隔离 Flow Rules（平台 {Platform}）", networkId, _platform.PlatformName);
    }

    /// <inheritdoc />
    public async Task AssignRoomTagAsync(string nodeId, uint roomTagValue, CancellationToken cancellationToken = default)
    {
        string url = $"{ControlUrl}/controller/network/{NetworkId}/member/{nodeId}";

        var member = new MemberConfig
        {
            Tags = new List<List<long>> { new() { RoomTagId, roomTagValue } },
        };

        string authToken = await _platform.ResolveAuthTokenAsync(_options.AuthToken, cancellationToken).ConfigureAwait(false);
        await PostAsync(url, member, authToken, cancellationToken).ConfigureAwait(false);
        _logger.LogInformation("已给节点 {NodeId} 打上房间 Tag {Tag}", nodeId, roomTagValue);
    }

    /// <inheritdoc />
    public async Task ClearRoomTagAsync(string nodeId, CancellationToken cancellationToken = default)
    {
        string url = $"{ControlUrl}/controller/network/{NetworkId}/member/{nodeId}";

        // Tag 值置 0 即视为「无房间」；Flow Rules 据此拒绝该节点跨房间互通。
        var member = new MemberConfig
        {
            Tags = new List<List<long>> { new() { RoomTagId, 0L } },
        };

        string authToken = await _platform.ResolveAuthTokenAsync(_options.AuthToken, cancellationToken).ConfigureAwait(false);
        await PostAsync(url, member, authToken, cancellationToken).ConfigureAwait(false);
        _logger.LogInformation("已清除节点 {NodeId} 的房间 Tag", nodeId);
    }

    private string ControlUrl
        => string.IsNullOrWhiteSpace(_options.ControlUrl)
            ? _platform.DefaultControlUrl
            : _options.ControlUrl.TrimEnd('/');

    /// <summary>构建房间隔离 Flow Rules 的 rulesSource DSL 文本。</summary>
    private string BuildRulesSource()
    {
        // 1) 允许 ARP（节点互相发现、二层寻址）。
        // 2) 声明 room_id 标签。
        // 3) 双方 room_id 不都为 1（不在同一房间）→ break，落到默认 drop。
        // 4) 其余流量（同房间）放行。
        string tagName = string.IsNullOrWhiteSpace(_options.RoomTagName) ? "room_id" : _options.RoomTagName;

        return
            $"accept ethertype arp;\n" +
            $"tag {tagName}\n  id {RoomTagId}\n  default 0\n;\n\n" +
            $"break not tor {tagName} 1;\n\n" +
            $"accept;";
    }

    private HttpClient CreateClient(string authToken)
    {
        HttpClient client = _httpClientFactory.CreateClient(nameof(SelfHostedZeroTierController));
        client.Timeout = TimeSpan.FromSeconds(15);
        client.DefaultRequestHeaders.Add("X-ZT1-Auth", authToken);
        return client;
    }

    private async Task PostAsync<T>(string url, T payload, string authToken, CancellationToken cancellationToken)
    {
        using HttpClient client = CreateClient(authToken);
        string json = JsonSerializer.Serialize(payload, JsonOptions);
        using var content = new StringContent(json, Encoding.UTF8, "application/json");
        using var response = await client.PostAsync(url, content, cancellationToken).ConfigureAwait(false);

        if (!response.IsSuccessStatusCode)
        {
            string detail = await response.Content.ReadAsStringAsync(cancellationToken).ConfigureAwait(false);
            throw new HttpRequestException($"ZeroTier Controller 调用失败（{(int)response.StatusCode}）：{detail}", null, response.StatusCode);
        }
    }

    private sealed class NetworkConfig
    {
        public string? RulesSource { get; set; }
    }

    private sealed class MemberConfig
    {
        public List<List<long>>? Tags { get; set; }
    }
}
