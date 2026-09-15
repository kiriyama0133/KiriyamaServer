namespace KiriyamaServer.Domain.Games;

/// <summary>
/// 一个可联机的游戏板块（实体）。是房间列表的顶层分类维度，
/// 客户端据此按「游戏板块」浏览对应房间。
/// </summary>
public sealed class Game
{
    /// <summary>游戏稳定标识（如 civ6）。</summary>
    public GameKey Key { get; }

    /// <summary>游戏显示名（如「文明 6」）。</summary>
    public string DisplayName { get; }

    public Game(GameKey key, string displayName)
    {
        if (string.IsNullOrWhiteSpace(displayName))
        {
            throw new DomainException("Game display name cannot be empty.");
        }

        Key = key;
        DisplayName = displayName.Trim();
    }
}
