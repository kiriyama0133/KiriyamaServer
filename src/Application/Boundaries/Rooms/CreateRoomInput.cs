using KiriyamaServer.Application.Exceptions;
using KiriyamaServer.Application.Interfaces;
using KiriyamaServer.Domain.Games;

namespace KiriyamaServer.Application.Boundaries.Rooms;

/// <summary>创建房间用例的输入。</summary>
public sealed class CreateRoomInput : IInputType
{
    public string Name { get; }
    public string HostName { get; }
    public GameKey GameKey { get; }
    public string? Password { get; }
    public int MaxPlayers { get; }

    public CreateRoomInput(string name, string hostName, GameKey gameKey, string? password, int maxPlayers)
    {
        Name = string.IsNullOrWhiteSpace(name)
            ? throw new InputValidationException("Room name cannot be empty.")
            : name;

        HostName = string.IsNullOrWhiteSpace(hostName)
            ? throw new InputValidationException("Host name cannot be empty.")
            : hostName;

        GameKey = gameKey;

        Password = string.IsNullOrWhiteSpace(password) ? null : password;

        MaxPlayers = maxPlayers < 0
            ? throw new InputValidationException("Max players cannot be negative.")
            : maxPlayers;
    }
}
