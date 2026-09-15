using KiriyamaServer.Application.Interfaces;
using KiriyamaServer.Domain.Games;

namespace KiriyamaServer.Application.Boundaries.Rooms;

/// <summary>列出房间用例的输入（可按游戏板块过滤）。</summary>
public sealed class ListRoomsInput : IInputType
{
    public static ListRoomsInput Instance { get; } = new();

    /// <summary>按游戏板块过滤；null 表示列出全部房间。</summary>
    public GameKey? GameKey { get; private init; }

    private ListRoomsInput()
    {
    }

    /// <summary>构造一个按游戏板块过滤的输入。</summary>
    public static ListRoomsInput ForGame(GameKey gameKey) => new() { GameKey = gameKey };
}
