using KiriyamaServer.Application.Exceptions;
using KiriyamaServer.Application.Interfaces;
using KiriyamaServer.Domain.Rooms;

namespace KiriyamaServer.Application.Boundaries.Rooms;

/// <summary>加入房间用例的输入。</summary>
public sealed class JoinRoomInput : IInputType
{
    public RoomId RoomId { get; }
    public string Nickname { get; }
    public string NodeId { get; }
    public string VirtualIp { get; }
    public string? Password { get; }

    public JoinRoomInput(RoomId roomId, string nickname, string nodeId, string virtualIp, string? password)
    {
        RoomId = roomId;

        Nickname = string.IsNullOrWhiteSpace(nickname)
            ? throw new InputValidationException("Nickname cannot be empty.")
            : nickname;

        NodeId = string.IsNullOrWhiteSpace(nodeId)
            ? throw new InputValidationException("Node id cannot be empty.")
            : nodeId;

        VirtualIp = string.IsNullOrWhiteSpace(virtualIp)
            ? throw new InputValidationException("Virtual ip cannot be empty.")
            : virtualIp;

        Password = string.IsNullOrWhiteSpace(password) ? null : password;
    }
}
