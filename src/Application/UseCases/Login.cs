using KiriyamaServer.Application.Boundaries.Auth;
using KiriyamaServer.Application.Interfaces;
using KiriyamaServer.Application.Repositories;
using KiriyamaServer.Application.Services;
using KiriyamaServer.Domain.Users;

namespace KiriyamaServer.Application.UseCases;

/// <summary>登录：校验密码，签发一次性 PKCE 授权码。</summary>
public sealed class Login(
    IUserRepository userRepository,
    IPasswordHasher passwordHasher,
    ITokenService tokenService,
    ILoginOutputPort outputPort)
    : IUseCase<LoginInput>
{
    private readonly IUserRepository _userRepository = userRepository ?? throw new ArgumentNullException(nameof(userRepository));
    private readonly IPasswordHasher _passwordHasher = passwordHasher ?? throw new ArgumentNullException(nameof(passwordHasher));
    private readonly ITokenService _tokenService = tokenService ?? throw new ArgumentNullException(nameof(tokenService));
    private readonly ILoginOutputPort _outputPort = outputPort ?? throw new ArgumentNullException(nameof(outputPort));

    public async Task ExecuteAsync(LoginInput input, CancellationToken cancellationToken = default)
    {
        User? user = await _userRepository.FindByEmailAsync(input.Email, cancellationToken);
        if (user is null || !_passwordHasher.Verify(input.Password, user.PasswordHash))
        {
            // 统一提示，避免泄露「邮箱是否存在」。
            _outputPort.Error("Invalid email or password.");
            return;
        }

        string code = _tokenService.GenerateAuthorizationCode();
        DateTime expiresAt = DateTime.UtcNow.AddMinutes(5);
        user.IssueAuthorizationCode(code, input.CodeChallenge, expiresAt);

        await _userRepository.UpdateAsync(user, cancellationToken);

        _outputPort.Standard(new LoginOutput(code, expiresAt, user.DisplayName));
    }
}
