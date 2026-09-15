namespace KiriyamaServer.Application.Services;

/// <summary>
/// 密码哈希服务（驱动端口）。用于生成与校验密码哈希（如 PBKDF2 / bcrypt）。
/// </summary>
public interface IPasswordHasher
{
    /// <summary>对明文密码生成哈希。</summary>
    string Hash(string plainText);

    /// <summary>校验明文密码是否与哈希匹配。</summary>
    bool Verify(string plainText, string hash);
}
