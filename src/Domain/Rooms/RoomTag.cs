using System.Security.Cryptography;

namespace KiriyamaServer.Domain.Rooms;

/// <summary>
/// 房间对应的 ZeroTier Tag 值（值对象）。
///
/// ZeroTier Tag 值是一个 0~4294967295 的无符号整数（32 位），
/// 服务端为每个房间生成一个唯一的 Tag 值，供 Flow Rules 做策略匹配。
/// </summary>
public readonly record struct RoomTag
{
    private readonly uint _value;

    private RoomTag(uint value) => _value = value;

    /// <summary>Tag 的 32 位整数值。</summary>
    public uint Value => _value;

    /// <summary>Tag 的十进制字符串表示（用于 JSON 序列化与 Controller 交互）。</summary>
    public string StringValue => _value.ToString();

    /// <summary>生成一个随机的新 Tag 值（避免与其它房间冲突）。</summary>
    public static RoomTag New()
    {
        Span<byte> bytes = stackalloc byte[4];
        RandomNumberGenerator.Fill(bytes);
        return new RoomTag(BitConverter.ToUInt32(bytes));
    }

    /// <summary>从已有的 uint 还原 Tag 值。</summary>
    public static RoomTag From(uint value) => new(value);

    /// <summary>从十进制字符串还原 Tag 值。</summary>
    public static RoomTag FromString(string value)
    {
        if (!uint.TryParse(value, out uint parsed))
        {
            throw new DomainException($"Invalid room tag value: {value}.");
        }

        return new RoomTag(parsed);
    }

    public override string ToString() => _value.ToString();
}
