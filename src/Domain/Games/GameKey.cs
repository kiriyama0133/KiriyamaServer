namespace KiriyamaServer.Domain.Games;

/// <summary>
/// 游戏的稳定标识（值对象）。
///
/// 用小写短键（如 <c>civ6</c>）作为游戏的唯一标识，
/// 与显示名（如「文明 6」）分离，便于客户端按板块路由。
/// </summary>
public readonly record struct GameKey
{
    private readonly string _value;

    private GameKey(string value) => _value = value;

    /// <summary>游戏标识的字符串表示（小写短键）。</summary>
    public string Value => _value;

    /// <summary>从已有字符串还原游戏标识（统一转小写）。</summary>
    public static GameKey From(string value)
    {
        if (string.IsNullOrWhiteSpace(value))
        {
            throw new DomainException("Game key cannot be empty.");
        }

        return new GameKey(value.Trim().ToLowerInvariant());
    }

    public override string ToString() => _value;
}
