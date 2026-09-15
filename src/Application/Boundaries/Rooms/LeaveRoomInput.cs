using KiriyamaServer.Application.Exceptions;
using KiriyamaServer.Application.Interfaces;
using KiriyamaServer.Domain.Rooms;

namespace KiriyamaServer.Application.Boundaries.Rooms;

/// <summary>离开房间用例的输入。</summary>
public sealed class LeaveRoomInput : IInputType
{
    public RoomId RoomId { get; }
    public string NodeId { get; }

    public LeaveRoomInput(RoomId roomId, string nodeId)
    {
        RoomId = roomId;

        NodeId = string.IsNullOrWhiteSpace(nodeId)
            ? throw new InputValidationException("Node id cannot be empty.")
            : nodeId;
    }
}
