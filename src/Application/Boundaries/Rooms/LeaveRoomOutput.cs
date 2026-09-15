using KiriyamaServer.Application.Interfaces;

namespace KiriyamaServer.Application.Boundaries.Rooms;

/// <summary>离开房间用例的输出。</summary>
public sealed class LeaveRoomOutput : IOutputType
{
    public bool RoomClosed { get; }

    public LeaveRoomOutput(bool roomClosed)
    {
        RoomClosed = roomClosed;
    }
}
