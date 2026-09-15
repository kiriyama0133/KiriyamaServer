using KiriyamaServer.Application.Interfaces;

namespace KiriyamaServer.Application.Boundaries.Games;

/// <summary>列出游戏板块用例的输入（无参数）。</summary>
public sealed class ListGamesInput : IInputType
{
    public static ListGamesInput Instance { get; } = new();

    private ListGamesInput()
    {
    }
}
