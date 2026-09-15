using System.ComponentModel.DataAnnotations;

namespace KiriyamaServer.WebApi.UseCases.Rooms;

/// <summary>
/// 房间的展示 DTO（camelCase 序列化，对齐客户端 IRelayServerClient 协议）。
/// </summary>
public sealed class RoomDto
{
    /// <summary>房间标识（6 位短码）。</summary>
    [Required]
    public string Id { get; }

    /// <summary>房间名。</summary>
    [Required]
    public string Name { get; }

    /// <summary>房主昵称。</summary>
    [Required]
    public string Host { get; }

    /// <summary>房间归属的游戏板块标识（如 civ6）。</summary>
    [Required]
    public string GameKey { get; }

    /// <summary>当前玩家数。</summary>
    [Required]
    public int PlayerCount { get; }

    /// <summary>最大玩家数（0 表示不限）。</summary>
    [Required]
    public int MaxPlayers { get; }

    /// <summary>是否设有密码。</summary>
    [Required]
    public bool HasPassword { get; }

    public RoomDto(string id, string name, string host, string gameKey, int playerCount, int maxPlayers, bool hasPassword)
    {
        Id = id;
        Name = name;
        Host = host;
        GameKey = gameKey;
        PlayerCount = playerCount;
        MaxPlayers = maxPlayers;
        HasPassword = hasPassword;
    }
}
