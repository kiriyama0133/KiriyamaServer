using KiriyamaServer.Application.Exceptions;
using KiriyamaServer.Application.Interfaces;
using KiriyamaServer.Domain.Games;

namespace KiriyamaServer.Application.Boundaries.Rooms;

/// <summary>创建房间用例的输入。</summary>
public sealed class CreateRoomInput : IInputType
{
    public string Name { get; }
    public string HostName { get; }

    /// <summary>房主的节点 ID（用于转让房主与「房主退出即销毁房间」的判定）。</summary>
    public string HostNodeId { get; }

    public GameKey GameKey { get; }
    public string? Password { get; }
    public int MaxPlayers { get; }

    public CreateRoomInput(string name, string hostName, string? hostNodeId, GameKey gameKey, string? password, int maxPlayers)
    {
        Name = string.IsNullOrWhiteSpace(name)
            ? throw new InputValidationException("Room name cannot be empty.")
            : name;

        HostName = string.IsNullOrWhiteSpace(hostName)
            ? throw new InputValidationException("Host name cannot be empty.")
            : hostName;

        // 允许为空：旧版客户端不上报节点 ID，由第一个加入者认领房主身份。
        HostNodeId = hostNodeId?.Trim() ?? string.Empty;

        GameKey = gameKey;

        Password = string.IsNullOrWhiteSpace(password) ? null : password;

        MaxPlayers = maxPlayers < 0
            ? throw new InputValidationException("Max players cannot be negative.")
            : maxPlayers;
    }
}
