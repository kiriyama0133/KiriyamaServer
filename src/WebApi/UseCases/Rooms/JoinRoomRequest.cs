using System.ComponentModel.DataAnnotations;

namespace KiriyamaServer.WebApi.UseCases.Rooms;

/// <summary>加入房间请求。</summary>
public sealed class JoinRoomRequest
{
    /// <summary>玩家昵称。</summary>
    [Required]
    public required string Nickname { get; init; }

    /// <summary>玩家在 ZeroTier 网络里的节点 ID（10 位十六进制，用于服务端打 Tag）。</summary>
    [Required]
    public required string NodeId { get; init; }

    /// <summary>玩家在 ZeroTier 虚拟网中的节点 IP。</summary>
    [Required]
    public required string VirtualIp { get; init; }

    /// <summary>房间密码（无密码房间可省略）。</summary>
    public string? Password { get; init; }
}
