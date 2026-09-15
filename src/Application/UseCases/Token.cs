using KiriyamaServer.Application.Boundaries.Auth;
using KiriyamaServer.Application.Interfaces;
using KiriyamaServer.Application.Repositories;
using KiriyamaServer.Application.Services;
using KiriyamaServer.Domain.Users;

namespace KiriyamaServer.Application.UseCases;

/// <summary>
/// OAuth token 端点：支持 <c>authorization_code</c>（PKCE）与 <c>refresh_token</c> 两种 grant。
/// </summary>
public sealed class Token(
    IUserRepository userRepository,
    ITokenService tokenService,
    ITokenOutputPort outputPort)
    : IUseCase<TokenInput>
{
    private readonly IUserRepository _userRepository = userRepository ?? throw new ArgumentNullException(nameof(userRepository));
    private readonly ITokenService _tokenService = tokenService ?? throw new ArgumentNullException(nameof(tokenService));
    private readonly ITokenOutputPort _outputPort = outputPort ?? throw new ArgumentNullException(nameof(outputPort));

    public async Task ExecuteAsync(TokenInput input, CancellationToken cancellationToken = default)
    {
        switch (input.GrantType)
        {
            case "authorization_code":
                await HandleAuthorizationCodeAsync(input, cancellationToken);
                break;
            case "refresh_token":
                await HandleRefreshTokenAsync(input, cancellationToken);
                break;
            default:
                _outputPort.Error("Unsupported grant type.");
                break;
        }
    }

    private async Task HandleAuthorizationCodeAsync(TokenInput input, CancellationToken cancellationToken)
    {
        if (string.IsNullOrWhiteSpace(input.Code))
        {
            _outputPort.Error("Code is required.");
            return;
        }

        User? user = await _userRepository.FindByAuthorizationCodeAsync(input.Code, cancellationToken);
        if (user is null)
        {
            _outputPort.Error("Invalid or expired authorization code.");
            return;
        }

        string codeChallenge = _tokenService.ComputeCodeChallenge(input.CodeVerifier);
        if (user.ConsumeAuthorizationCode(input.Code, codeChallenge, DateTime.UtcNow) is null)
        {
            _outputPort.Error("Invalid code verifier.");
            return;
        }

        await _userRepository.UpdateAsync(user, cancellationToken);
        await IssueTokensAsync(user, input.ClientId, cancellationToken);
    }

    private async Task HandleRefreshTokenAsync(TokenInput input, CancellationToken cancellationToken)
    {
        if (string.IsNullOrWhiteSpace(input.RefreshToken))
        {
            _outputPort.Error("Refresh token is required.");
            return;
        }

        string tokenHash = HashToken(input.RefreshToken);
        User? user = await _userRepository.FindByRefreshTokenAsync(tokenHash, cancellationToken);
        if (user is null)
        {
            _outputPort.Error("Invalid or expired refresh token.");
            return;
        }

        RefreshToken? existing = user.FindActiveRefreshToken(tokenHash, DateTime.UtcNow);
        if (existing is null)
        {
            _outputPort.Error("Invalid or expired refresh token.");
            return;
        }

        // 撤销旧令牌（轮换），再发新令牌。
        user.RevokeRefreshToken(tokenHash);

        await IssueTokensAsync(user, input.ClientId, cancellationToken);
    }

    private async Task IssueTokensAsync(User user, ClientId clientId, CancellationToken cancellationToken)
    {
        string accessToken = _tokenService.IssueAccessToken(user, clientId);

        (string plain, string hash) = _tokenService.IssueRefreshToken();
        user.IssueRefreshToken(hash, DateTime.UtcNow.AddSeconds(_tokenService.RefreshTokenLifetimeSeconds));

        await _userRepository.UpdateAsync(user, cancellationToken);

        _outputPort.Standard(new TokenOutput(accessToken, "Bearer", _tokenService.AccessTokenLifetimeSeconds, plain));
    }

    /// <summary>对刷新令牌明文做哈希（存库前），与签发时一致。</summary>
    private string HashToken(string plain)
        => _tokenService.ComputeCodeChallenge(plain);
}
