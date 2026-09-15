using KiriyamaServer.Application.Interfaces;

namespace KiriyamaServer.Application.Boundaries.Auth;

/// <summary>注册用例的输出。</summary>
public sealed class RegisterOutput : IOutputType
{
    public Guid UserId { get; }
    public string Email { get; }
    public string DisplayName { get; }

    public RegisterOutput(Guid userId, string email, string displayName)
    {
        UserId = userId;
        Email = email;
        DisplayName = displayName;
    }
}

/// <summary>注册用例的输出端口。</summary>
public interface IRegisterOutputPort : IErrorHandler
{
    void Standard(RegisterOutput output);
}
