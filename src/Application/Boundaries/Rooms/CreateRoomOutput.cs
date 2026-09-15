using KiriyamaServer.Application.Interfaces;

namespace KiriyamaServer.Application.Boundaries.Rooms;

/// <summary>创建房间用例的输出。</summary>
public sealed class CreateRoomOutput : IOutputType
{
    public RoomInfo Room { get; }

    public CreateRoomOutput(RoomInfo room)
    {
        Room = room;
    }
}
