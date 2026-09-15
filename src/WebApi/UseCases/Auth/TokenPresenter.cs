using KiriyamaServer.Application.Boundaries.Auth;
using Microsoft.AspNetCore.Mvc;

namespace KiriyamaServer.WebApi.UseCases.Auth;

/// <summary>令牌兑换用例的 Presenter。</summary>
public sealed class TokenPresenter : ITokenOutputPort
{
    public IActionResult? ViewModel { get; private set; }

    public void Error(string message)
    {
        ViewModel = new BadRequestObjectResult(new ProblemDetails
        {
            Title = "Token error",
            Detail = message
        });
    }

    public void Standard(TokenOutput output)
    {
        ViewModel = new OkObjectResult(new
        {
            output.AccessToken,
            output.TokenType,
            output.ExpiresIn,
            output.RefreshToken
        });
    }
}
