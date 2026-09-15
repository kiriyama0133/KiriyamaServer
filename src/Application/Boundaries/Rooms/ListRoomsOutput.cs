using KiriyamaServer.Application.Interfaces;

namespace KiriyamaServer.Application.Boundaries.Rooms;

/// <summary>单个房间的展示数据。</summary>
public sealed class RoomInfo
{
    public string Id { get; }
    public string Name { get; }
    public string Host { get; }
    public string GameKey { get; }
    public int PlayerCount { get; }
    public int MaxPlayers { get; }
    public bool HasPassword { get; }

    public RoomInfo(string id, string name, string host, string gameKey, int playerCount, int maxPlayers, bool hasPassword)
    {
        Id = id;
        Name = name;
        Host = host;
        GameKey = gameKey;
        PlayerCount = playerCount;
        MaxPlayers = maxPlayers;
        HasPassword = hasPassword;
    }
}

/// <summary>列出房间用例的输出。</summary>
public sealed class ListRoomsOutput : IOutputType
{
    public IReadOnlyList<RoomInfo> Rooms { get; }

    public ListRoomsOutput(IReadOnlyList<RoomInfo> rooms)
    {
        Rooms = rooms;
    }
}
