using KiriyamaServer.Application.Boundaries.Auth;
using KiriyamaServer.Application.Interfaces;
using KiriyamaServer.Application.Repositories;
using KiriyamaServer.Application.Services;
using KiriyamaServer.Domain.Users;

namespace KiriyamaServer.Application.UseCases;

/// <summary>注册新用户。</summary>
public sealed class RegisterUser(
    IUserRepository userRepository,
    IPasswordHasher passwordHasher,
    IRegisterOutputPort outputPort)
    : IUseCase<RegisterInput>
{
    private readonly IUserRepository _userRepository = userRepository ?? throw new ArgumentNullException(nameof(userRepository));
    private readonly IPasswordHasher _passwordHasher = passwordHasher ?? throw new ArgumentNullException(nameof(passwordHasher));
    private readonly IRegisterOutputPort _outputPort = outputPort ?? throw new ArgumentNullException(nameof(outputPort));

    public async Task ExecuteAsync(RegisterInput input, CancellationToken cancellationToken = default)
    {
        User? existing = await _userRepository.FindByEmailAsync(input.Email, cancellationToken);
        if (existing is not null)
        {
            _outputPort.Error("An account with this email already exists.");
            return;
        }

        string passwordHash = _passwordHasher.Hash(input.Password);
        var user = new User(input.Email, passwordHash, input.DisplayName);

        await _userRepository.AddAsync(user, cancellationToken);

        _outputPort.Standard(new RegisterOutput(user.Id, user.Email.Value, user.DisplayName));
    }
}
