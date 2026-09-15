using KiriyamaServer.Application.Exceptions;
using KiriyamaServer.Application.Interfaces;
using KiriyamaServer.Domain.Users;

namespace KiriyamaServer.Application.Boundaries.Auth;

/// <summary>注册用例的输入。</summary>
public sealed class RegisterInput : IInputType
{
    public Email Email { get; }
    public string Password { get; }
    public string DisplayName { get; }

    public RegisterInput(string email, string password, string displayName)
    {
        if (string.IsNullOrWhiteSpace(password) || password.Length < 8)
        {
            throw new InputValidationException("Password must be at least 8 characters.");
        }

        Email = new Email(email);
        Password = password;
        DisplayName = displayName;
    }
}
