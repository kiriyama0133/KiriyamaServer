using KiriyamaServer.Domain.Games;

namespace KiriyamaServer.Domain.Rooms;

/// <summary>
/// 游戏房间（聚合根）。封装房间的创建、加入、离开、密码校验与人数上限。
///
/// 每个房间对应一个 ZeroTier Tag 值（<see cref="RoomTag"/>），
/// 玩家加入房间后由服务端调用 Controller 给该玩家的节点打上此 Tag，
/// 再由网络 Flow Rules 按 Tag 值控制「同房间互通、跨房间隔离」。
/// </summary>
public sealed class GameRoom
{
    private readonly List<Player> _players = new();

    /// <summary>房间标识。</summary>
    public RoomId Id { get; }

    /// <summary>房间名。</summary>
    public string Name { get; }

    /// <summary>房主昵称（转让房主时会变，所以可变）。</summary>
    public string HostName { get; private set; }

    /// <summary>
    /// 房主的节点 ID。房主身份以节点 ID 为准（昵称可能重名，节点 ID 唯一）。
    /// 创建时未指定（旧客户端）则为空串，由第一个加入者认领。
    /// </summary>
    public string HostNodeId { get; private set; }

    /// <summary>房间归属的游戏板块。</summary>
    public GameKey GameKey { get; }

    /// <summary>房间对应的 ZeroTier Tag 值（创建时生成，记录到房间数据）。</summary>
    public RoomTag RoomTag { get; }

    /// <summary>房间密码（可空，表示无密码）。</summary>
    public RoomPassword? Password { get; }

    /// <summary>最大玩家数（0 表示不限制）。</summary>
    public int MaxPlayers { get; }

    /// <summary>创建时间。</summary>
    public DateTime CreatedAt { get; }

    /// <summary>当前房间内玩家集合（只读视图）。</summary>
    public IReadOnlyList<Player> Players => _players;

    /// <summary>当前玩家人数。</summary>
    public int PlayerCount => _players.Count;

    /// <summary>是否设有密码。</summary>
    public bool HasPassword => Password is not null;

    /// <summary>房间是否已满。</summary>
    public bool IsFull => MaxPlayers > 0 && _players.Count >= MaxPlayers;

    public GameRoom(RoomId id, string name, string hostName, string hostNodeId, GameKey gameKey, RoomTag roomTag, RoomPassword? password, int maxPlayers)
    {
        if (string.IsNullOrWhiteSpace(name))
        {
            throw new DomainException("Room name cannot be empty.");
        }

        if (string.IsNullOrWhiteSpace(hostName))
        {
            throw new DomainException("Host name cannot be empty.");
        }

        if (maxPlayers < 0)
        {
            throw new DomainException("Max players cannot be negative.");
        }

        Id = id;
        Name = name.Trim();
        HostName = hostName.Trim();

        // 允许为空：旧版客户端创建房间时不上报节点 ID，由第一个加入者认领房主身份。
        HostNodeId = string.IsNullOrWhiteSpace(hostNodeId) ? string.Empty : hostNodeId.Trim();

        GameKey = gameKey;
        RoomTag = roomTag;
        Password = password;
        MaxPlayers = maxPlayers;
        CreatedAt = DateTime.UtcNow;
    }

    /// <summary>校验密码；无密码房间直接通过。</summary>
    public bool IsPasswordValid(string? plainText)
    {
        if (Password is null)
        {
            return true;
        }

        return Password.Verify(plainText ?? string.Empty);
    }

    /// <summary>
    /// 让一名玩家加入房间。同一节点（NodeId）重复加入时刷新原记录而不是新增，
    /// 保证一个节点在房间里只占一个名额（上次 leave 丢失或 join 重试不会产生重复成员）。
    /// 房间满则抛领域异常（自己的旧记录会先腾出名额，重进不受影响）。
    /// </summary>
    public Player Join(string nickname, string nodeId, string virtualIp)
    {
        RemoveByNodeId(nodeId);

        if (IsFull)
        {
            throw new DomainException("The room is full.");
        }

        var player = new Player(nickname, nodeId, virtualIp);
        _players.Add(player);

        // 创建房间时未上报节点 ID（旧客户端）时，由第一个加入者认领房主身份。
        if (string.IsNullOrWhiteSpace(HostNodeId))
        {
            HostNodeId = player.NodeId;
            HostName = player.Nickname;
        }
        else if (IsHost(player.NodeId))
        {
            // 房主改昵称后重新加入：同步房主显示名。
            HostName = player.Nickname;
        }

        return player;
    }

    /// <summary>判断指定节点是不是房主。</summary>
    public bool IsHost(string nodeId)
        => !string.IsNullOrWhiteSpace(HostNodeId)
            && string.Equals(HostNodeId, nodeId?.Trim(), StringComparison.OrdinalIgnoreCase);

    /// <summary>
    /// 转让房主：只有当前房主可以发起，目标必须是房间内成员。
    /// 成功返回成为新房主的玩家（房主身份按节点 ID 转移，昵称一并更新）。
    /// </summary>
    public Player TransferHost(string requesterNodeId, string targetNodeId)
    {
        if (!IsHost(requesterNodeId))
        {
            throw new DomainException("Only the current host can transfer ownership.");
        }

        string target = (targetNodeId ?? string.Empty).Trim();

        if (target.Length == 0)
        {
            throw new DomainException("The target node id cannot be empty.");
        }

        Player? player = _players.FirstOrDefault(p =>
            string.Equals(p.NodeId, target, StringComparison.OrdinalIgnoreCase));

        if (player is null)
        {
            throw new DomainException("The target player is not in this room.");
        }

        if (IsHost(player.NodeId))
        {
            throw new DomainException("The target player is already the host.");
        }

        HostNodeId = player.NodeId;
        HostName = player.Nickname;
        return player;
    }

    /// <summary>让指定节点的所有成员记录离开房间（历史重复加入可能遗留多条）。返回移除的数量。</summary>
    public int LeaveByNodeId(string nodeId) => RemoveByNodeId(nodeId);

    /// <summary>移除指定节点的全部成员记录，返回移除数量。</summary>
    private int RemoveByNodeId(string nodeId)
    {
        return _players.RemoveAll(p =>
            string.Equals(p.NodeId, nodeId.Trim(), StringComparison.OrdinalIgnoreCase));
    }

    /// <summary>房间是否为空（无任何玩家）。</summary>
    public bool IsEmpty => _players.Count == 0;
}
