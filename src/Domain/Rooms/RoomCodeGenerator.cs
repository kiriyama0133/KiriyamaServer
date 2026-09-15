namespace KiriyamaServer.Domain.Rooms;

/// <summary>
/// 房间短码生成器：生成一个易读易输入的 6 位大写字母数字房间号。
/// </summary>
internal static class RoomCodeGenerator
{
    // 去掉易混淆字符（0/O、1/I/L）。
    private const string Alphabet = "ABCDEFGHJKMNPQRSTUVWXYZ23456789";
    private const int Length = 6;

    private static readonly Random Random = new();

    /// <summary>生成一个 6 位房间号（碰撞处理由房间仓库负责）。</summary>
    public static string Next()
    {
        Span<char> buffer = stackalloc char[Length];
        for (int i = 0; i < Length; i++)
        {
            buffer[i] = Alphabet[Random.Next(Alphabet.Length)];
        }

        return new string(buffer);
    }
}
