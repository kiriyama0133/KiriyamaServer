using System.ComponentModel.DataAnnotations;

namespace KiriyamaServer.WebApi.UseCases.Rooms;

/// <summary>离开房间请求。</summary>
public sealed class LeaveRoomRequest
{
    /// <summary>玩家在 ZeroTier 网络里的节点 ID（用于服务端清除 Tag）。</summary>
    [Required]
    public required string NodeId { get; init; }
}
