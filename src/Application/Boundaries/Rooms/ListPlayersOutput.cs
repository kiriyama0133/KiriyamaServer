using KiriyamaServer.Application.Interfaces;

namespace KiriyamaServer.Application.Boundaries.Rooms;

/// <summary>房间内一名玩家的展示数据。</summary>
public sealed class PlayerInfo
{
    /// <summary>玩家连接标识。</summary>
    public Guid PlayerId { get; }

    /// <summary>玩家昵称（客户端只显示这个，不显示邮箱/节点 ID）。</summary>
    public string Nickname { get; }

    /// <summary>玩家在 ZeroTier 网络里的节点 ID（用于客户端 Ping 定位，非展示必需）。</summary>
    public string NodeId { get; }

    /// <summary>玩家在虚拟网中的 IP（客户端用它做延迟探测）。</summary>
    public string VirtualIp { get; }

    public PlayerInfo(Guid playerId, string nickname, string nodeId, string virtualIp)
    {
        PlayerId = playerId;
        Nickname = nickname;
        NodeId = nodeId;
        VirtualIp = virtualIp;
    }
}

/// <summary>列出房间内玩家用例的输出。</summary>
public sealed class ListPlayersOutput : IOutputType
{
    public string RoomId { get; }
    public string RoomName { get; }
    public IReadOnlyList<PlayerInfo> Players { get; }

    public ListPlayersOutput(string roomId, string roomName, IReadOnlyList<PlayerInfo> players)
    {
        RoomId = roomId;
        RoomName = roomName;
        Players = players;
    }
}

/// <summary>列出房间内玩家用例的输出端口。</summary>
public interface IListPlayersOutputPort : IErrorHandler
{
    void Standard(ListPlayersOutput output);

    /// <summary>房间不存在。</summary>
    void NotFound();
}
