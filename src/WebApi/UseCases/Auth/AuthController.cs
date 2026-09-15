using KiriyamaServer.Application.Boundaries.Auth;
using KiriyamaServer.Application.Interfaces;
using Microsoft.AspNetCore.Mvc;

namespace KiriyamaServer.WebApi.UseCases.Auth;

/// <summary>
/// 鉴权控制器：注册、登录（PKCE 授权码）、OAuth token 兑换。
/// </summary>
[Route("api/auth")]
[ApiController]
public sealed class AuthController : ControllerBase
{
    private readonly IUseCase<RegisterInput> _registerUseCase;
    private readonly RegisterPresenter _registerPresenter;
    private readonly IUseCase<LoginInput> _loginUseCase;
    private readonly LoginPresenter _loginPresenter;
    private readonly IUseCase<TokenInput> _tokenUseCase;
    private readonly TokenPresenter _tokenPresenter;

    public AuthController(
        IUseCase<RegisterInput> registerUseCase,
        RegisterPresenter registerPresenter,
        IUseCase<LoginInput> loginUseCase,
        LoginPresenter loginPresenter,
        IUseCase<TokenInput> tokenUseCase,
        TokenPresenter tokenPresenter)
    {
        _registerUseCase = registerUseCase;
        _registerPresenter = registerPresenter;
        _loginUseCase = loginUseCase;
        _loginPresenter = loginPresenter;
        _tokenUseCase = tokenUseCase;
        _tokenPresenter = tokenPresenter;
    }

    /// <summary>注册新用户。</summary>
    [HttpPost("register")]
    [ProducesResponseType(StatusCodes.Status201Created)]
    [ProducesResponseType(StatusCodes.Status409Conflict)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    public async Task<IActionResult?> RegisterAsync([FromBody] RegisterRequest request, CancellationToken cancellationToken = default)
    {
        var input = new RegisterInput(request.Email, request.Password, request.DisplayName ?? string.Empty);
        await _registerUseCase.ExecuteAsync(input, cancellationToken);
        return _registerPresenter.ViewModel;
    }

    /// <summary>登录：校验密码，返回一次性 PKCE 授权码。</summary>
    [HttpPost("login")]
    [ProducesResponseType(StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status401Unauthorized)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    public async Task<IActionResult?> LoginAsync([FromBody] LoginRequest request, CancellationToken cancellationToken = default)
    {
        var input = new LoginInput(request.Email, request.Password, request.CodeChallenge);
        await _loginUseCase.ExecuteAsync(input, cancellationToken);
        return _loginPresenter.ViewModel;
    }

    /// <summary>
    /// OAuth token 端点：grant_type=authorization_code（PKCE）或 refresh_token。
    /// 请求体为 application/x-www-form-urlencoded。
    /// </summary>
    [HttpPost("token")]
    [Consumes("application/x-www-form-urlencoded")]
    [ProducesResponseType(StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    public async Task<IActionResult?> TokenAsync([FromForm] TokenRequest request, CancellationToken cancellationToken = default)
    {
        var input = new TokenInput(request.GrantType, request.Code ?? string.Empty, request.CodeVerifier ?? string.Empty, request.RefreshToken, request.ClientId);
        await _tokenUseCase.ExecuteAsync(input, cancellationToken);
        return _tokenPresenter.ViewModel;
    }
}
