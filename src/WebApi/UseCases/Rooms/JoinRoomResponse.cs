using System.ComponentModel.DataAnnotations;

namespace KiriyamaServer.WebApi.UseCases.Rooms;

/// <summary>加入房间响应。</summary>
public sealed class JoinRoomResponse
{
    /// <summary>玩家在房间内的连接标识。</summary>
    [Required]
    public Guid PlayerId { get; }

    /// <summary>房间标识。</summary>
    [Required]
    public string RoomId { get; }

    public JoinRoomResponse(Guid playerId, string roomId)
    {
        PlayerId = playerId;
        RoomId = roomId;
    }
}
