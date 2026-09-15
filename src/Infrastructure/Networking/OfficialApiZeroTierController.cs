using System.Net.Http.Headers;
using System.Text;
using System.Text.Json;
using KiriyamaServer.Application.Services;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;

namespace KiriyamaServer.Infrastructure.Networking;

/// <summary>
/// 官方 ZeroTier Controller 实现（占位）：对接 my.zerotier.com 的 REST API
/// （<c>https://api.zerotier.com</c>，Bearer Token 认证），管理网络的
/// member tags / capabilities / rules。
///
/// 当前仅提供接口骨架与鉴权头拼装，具体网络/成员的字段映射与官方 API 完全对齐后再启用。
/// </summary>
public sealed class OfficialApiZeroTierController(
    IOptions<ZeroTierControllerOptions> options,
    IHttpClientFactory httpClientFactory,
    ILogger<OfficialApiZeroTierController> logger) : IZeroTierController
{
    private const int RoomTagId = 1;
    private static readonly JsonSerializerOptions JsonOptions = new() { PropertyNamingPolicy = JsonNamingPolicy.CamelCase };

    private readonly ZeroTierControllerOptions _options = options?.Value ?? throw new ArgumentNullException(nameof(options));
    private readonly IHttpClientFactory _httpClientFactory = httpClientFactory ?? throw new ArgumentNullException(nameof(httpClientFactory));
    private readonly ILogger<OfficialApiZeroTierController> _logger = logger ?? throw new ArgumentNullException(nameof(logger));

    private string NetworkId => _options.NetworkId
        ?? throw new InvalidOperationException("ZeroTier:NetworkId 未配置。");

    /// <inheritdoc />
    public Task EnsureNetworkAsync(CancellationToken cancellationToken = default)
    {
        // 官方 API 通过 PUT /api/v1/network/{networkId} 下发 config.tags（结构化数组）
        // 与 config.rules（规则数组），与自建 Controller 的 rulesSource DSL 文本不同。
        // 此处预留实现；启用时按官方契约填充。
        _logger.LogWarning("官方 Controller 后端尚未实现网络策略下发（NetworkId={NetworkId}）", NetworkId);
        return Task.CompletedTask;
    }

    /// <inheritdoc />
    public async Task AssignRoomTagAsync(string nodeId, uint roomTagValue, CancellationToken cancellationToken = default)
    {
        string url = $"{_options.ApiBaseUrl.TrimEnd('/')}/api/v1/network/{NetworkId}/member/{nodeId}";
        var payload = new { Tags = new List<List<long>> { new() { RoomTagId, roomTagValue } } };
        await PostAsync(url, payload, cancellationToken).ConfigureAwait(false);
    }

    /// <inheritdoc />
    public async Task ClearRoomTagAsync(string nodeId, CancellationToken cancellationToken = default)
    {
        string url = $"{_options.ApiBaseUrl.TrimEnd('/')}/api/v1/network/{NetworkId}/member/{nodeId}";
        var payload = new { Tags = new List<List<long>> { new() { RoomTagId, 0L } } };
        await PostAsync(url, payload, cancellationToken).ConfigureAwait(false);
    }

    private async Task PostAsync<T>(string url, T payload, CancellationToken cancellationToken)
    {
        using HttpClient client = _httpClientFactory.CreateClient(nameof(OfficialApiZeroTierController));
        client.Timeout = TimeSpan.FromSeconds(15);
        client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", _options.ApiToken);
        string json = JsonSerializer.Serialize(payload, JsonOptions);
        using var content = new StringContent(json, Encoding.UTF8, "application/json");
        using var response = await client.PostAsync(url, content, cancellationToken).ConfigureAwait(false);
        response.EnsureSuccessStatusCode();
    }
}
