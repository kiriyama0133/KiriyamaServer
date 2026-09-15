namespace KiriyamaServer.Domain.Rooms;

/// <summary>
/// 房间内的一名玩家（实体）。代表一个已加入房间的客户端节点。
/// </summary>
public sealed class Player
{
    /// <summary>玩家连接的唯一标识（由服务器分配）。</summary>
    public Guid Id { get; }

    /// <summary>玩家昵称。</summary>
    public string Nickname { get; }

    /// <summary>玩家在 ZeroTier 网络里的节点 ID（10 位十六进制，用于 Controller 打 Tag）。</summary>
    public string NodeId { get; }

    /// <summary>玩家在 ZeroTier 虚拟网中的节点 IP（记录用，非中继转发必需）。</summary>
    public string VirtualIp { get; }

    /// <summary>加入时间。</summary>
    public DateTime JoinedAt { get; }

    public Player(string nickname, string nodeId, string virtualIp)
    {
        if (string.IsNullOrWhiteSpace(nickname))
        {
            throw new DomainException("Player nickname cannot be empty.");
        }

        if (string.IsNullOrWhiteSpace(nodeId))
        {
            throw new DomainException("Player node id cannot be empty.");
        }

        if (string.IsNullOrWhiteSpace(virtualIp))
        {
            throw new DomainException("Player virtual ip cannot be empty.");
        }

        Id = Guid.NewGuid();
        Nickname = nickname.Trim();
        NodeId = nodeId.Trim();
        VirtualIp = virtualIp.Trim();
        JoinedAt = DateTime.UtcNow;
    }
}
