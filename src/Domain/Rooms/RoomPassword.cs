using System.Security.Cryptography;

namespace KiriyamaServer.Domain.Rooms;

/// <summary>
/// 房间密码（值对象）。始终以加盐哈希形式存储，从不保存明文。
/// </summary>
public sealed class RoomPassword
{
    private const int SaltSize = 16;
    private const int KeySize = 32;
    private const int Iterations = 100_000;

    private readonly byte[] _salt;
    private readonly byte[] _hash;

    private RoomPassword(byte[] salt, byte[] hash)
    {
        _salt = salt;
        _hash = hash;
    }

    /// <summary>从明文密码创建一个新的哈希密码。</summary>
    public static RoomPassword FromPlainText(string plainText)
    {
        if (string.IsNullOrEmpty(plainText))
        {
            throw new DomainException("Room password cannot be empty.");
        }

        byte[] salt = RandomNumberGenerator.GetBytes(SaltSize);
        byte[] hash = Rfc2898DeriveBytes.Pbkdf2(plainText, salt, Iterations, HashAlgorithmName.SHA256, KeySize);
        return new RoomPassword(salt, hash);
    }

    /// <summary>从已持久化的盐 + 哈希还原密码。</summary>
    public static RoomPassword FromStored(byte[] salt, byte[] hash)
        => new(salt, hash);

    /// <summary>校验一段明文是否与本密码匹配。</summary>
    public bool Verify(string plainText)
    {
        if (string.IsNullOrEmpty(plainText))
        {
            return false;
        }

        byte[] candidate = Rfc2898DeriveBytes.Pbkdf2(plainText, _salt, Iterations, HashAlgorithmName.SHA256, KeySize);
        return CryptographicOperations.FixedTimeEquals(candidate, _hash);
    }

    /// <summary>盐（Base64），用于持久化。</summary>
    public string Salt => Convert.ToBase64String(_salt);

    /// <summary>哈希（Base64），用于持久化。</summary>
    public string Hash => Convert.ToBase64String(_hash);
}
