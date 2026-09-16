using KiriyamaServer.Application.Boundaries.Auth;
using Microsoft.AspNetCore.Mvc;

namespace KiriyamaServer.WebApi.UseCases.Auth;

/// <summary>登录用例的 Presenter。</summary>
public sealed class LoginPresenter : ILoginOutputPort
{
    public IActionResult? ViewModel { get; private set; }

    public void Error(string message)
    {
        ViewModel = new UnauthorizedObjectResult(new ProblemDetails
        {
            Title = "Login failed",
            Detail = message
        });
    }

    public void Standard(LoginOutput output)
    {
        ViewModel = new OkObjectResult(new { output.Code, output.ExpiresAt, output.DisplayName });
    }
}
