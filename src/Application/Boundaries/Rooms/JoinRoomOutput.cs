using KiriyamaServer.Application.Interfaces;

namespace KiriyamaServer.Application.Boundaries.Rooms;

/// <summary>加入房间用例的输出。</summary>
public sealed class JoinRoomOutput : IOutputType
{
    /// <summary>玩家在房间内的连接标识。</summary>
    public Guid PlayerId { get; }

    /// <summary>房间标识。</summary>
    public string RoomId { get; }

    public JoinRoomOutput(Guid playerId, string roomId)
    {
        PlayerId = playerId;
        RoomId = roomId;
    }
}
