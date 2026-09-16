using KiriyamaServer.Application.Interfaces;
using KiriyamaServer.Domain.Rooms;

namespace KiriyamaServer.Application.Boundaries.Rooms;

/// <summary>列出房间内玩家用例的输入。</summary>
public sealed class ListPlayersInput : IInputType
{
    public RoomId RoomId { get; }

    public ListPlayersInput(RoomId roomId)
    {
        RoomId = roomId;
    }
}
