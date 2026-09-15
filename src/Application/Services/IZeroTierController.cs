namespace KiriyamaServer.Application.Services;

/// <summary>
/// ZeroTier Controller 抽象（驱动端口）。
///
/// 服务器节点通过它编程式管理 ZeroTier 网络的节点标签（Tag）与流量规则（Flow Rules），
/// 从而实现「房间隔离」：同一房间的节点共享同一个 Tag，网络 Flow Rules 按 Tag 值
/// 放行同房间内流量、阻断跨房间流量。ZeroTier 自身负责 P2P 打洞与失败时的回退中继，
/// 服务端无需再自建 UDP 中继。
/// </summary>
public interface IZeroTierController
{
    /// <summary>
    /// 确保网络存在并已下发房间隔离所需的 Tag 定义与 Flow Rules。
    ///
    /// 幂等操作：网络已存在时仅校验/补齐配置，不重复创建。
    /// </summary>
    Task EnsureNetworkAsync(CancellationToken cancellationToken = default);

    /// <summary>给指定节点打上某个房间的 Tag（节点加入房间时调用）。</summary>
    Task AssignRoomTagAsync(string nodeId, uint roomTagValue, CancellationToken cancellationToken = default);

    /// <summary>清除指定节点的房间 Tag（节点离开房间时调用）。</summary>
    Task ClearRoomTagAsync(string nodeId, CancellationToken cancellationToken = default);
}
