namespace KiriyamaServer.Domain.Users;

/// <summary>
/// 客户端标识（值对象）。用于区分不同的 OAuth 客户端（如桌面启动器 / Web）。
/// </summary>
public sealed class ClientId
{
    public string Value { get; }

    public ClientId(string value)
    {
        if (string.IsNullOrWhiteSpace(value))
        {
            throw new DomainException("Client id cannot be empty.");
        }

        Value = value.Trim();
    }

    public override string ToString() => Value;
}
