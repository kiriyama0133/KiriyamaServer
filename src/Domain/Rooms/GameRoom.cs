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

    /// <summary>房主昵称。</summary>
    public string HostName { get; }

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

    public GameRoom(RoomId id, string name, string hostName, GameKey gameKey, RoomTag roomTag, RoomPassword? password, int maxPlayers)
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

    /// <summary>让一名玩家加入房间。房间满则抛领域异常。</summary>
    public Player Join(string nickname, string nodeId, string virtualIp)
    {
        if (IsFull)
        {
            throw new DomainException("The room is full.");
        }

        var player = new Player(nickname, nodeId, virtualIp);
        _players.Add(player);
        return player;
    }

    /// <summary>让一名玩家离开房间。</summary>
    public Player? Leave(Guid playerId)
    {
        Player? player = _players.FirstOrDefault(p => p.Id == playerId);
        if (player is not null)
        {
            _players.Remove(player);
        }

        return player;
    }

    /// <summary>房间是否为空（无任何玩家）。</summary>
    public bool IsEmpty => _players.Count == 0;
}
