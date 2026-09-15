using KiriyamaServer.Application.Interfaces;

namespace KiriyamaServer.Application.Boundaries.Games;

/// <summary>单个游戏板块的展示数据。</summary>
public sealed class GameInfo
{
    public string Key { get; }
    public string DisplayName { get; }

    public GameInfo(string key, string displayName)
    {
        Key = key;
        DisplayName = displayName;
    }
}

/// <summary>列出游戏板块用例的输出。</summary>
public sealed class ListGamesOutput : IOutputType
{
    public IReadOnlyList<GameInfo> Games { get; }

    public ListGamesOutput(IReadOnlyList<GameInfo> games)
    {
        Games = games;
    }
}
