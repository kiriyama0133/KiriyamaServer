using System.ComponentModel.DataAnnotations;

namespace KiriyamaServer.WebApi.UseCases.Rooms;

/// <summary>创建房间请求。</summary>
public sealed class CreateRoomRequest
{
    /// <summary>房间名。</summary>
    [Required]
    public required string Name { get; init; }

    /// <summary>房主昵称。</summary>
    [Required]
    public required string HostName { get; init; }

    /// <summary>房主的 ZeroTier 节点 ID（用于转让房主与「房主退出即销毁房间」）。</summary>
    public string? HostNodeId { get; init; }

    /// <summary>房间归属的游戏板块标识（如 civ6）。</summary>
    [Required]
    public required string Game { get; init; }

    /// <summary>房间密码（空表示无密码）。</summary>
    public string? Password { get; init; }

    /// <summary>最大玩家数（0 表示不限）。</summary>
    public int MaxPlayers { get; init; }
}
