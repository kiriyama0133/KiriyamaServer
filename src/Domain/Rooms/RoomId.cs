namespace KiriyamaServer.Domain.Rooms;

/// <summary>
/// 游戏房间的唯一标识（值对象）。
/// </summary>
public readonly record struct RoomId
{
    private readonly string _value;

    private RoomId(string value) => _value = value;

    /// <summary>房间标识的字符串表示（如一个 8 位短码）。</summary>
    public string Value => _value;

    /// <summary>创建一个新的房间标识。</summary>
    public static RoomId New()
        => new(RoomCodeGenerator.Next());

    /// <summary>从已有字符串还原房间标识。</summary>
    public static RoomId From(string value)
    {
        if (string.IsNullOrWhiteSpace(value))
        {
            throw new DomainException("Room id cannot be empty.");
        }

        return new RoomId(value.Trim().ToUpperInvariant());
    }

    public override string ToString() => _value;
}
