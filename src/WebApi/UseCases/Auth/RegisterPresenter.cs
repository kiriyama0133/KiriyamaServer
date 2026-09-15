using KiriyamaServer.Application.Boundaries.Auth;
using Microsoft.AspNetCore.Mvc;

namespace KiriyamaServer.WebApi.UseCases.Auth;

/// <summary>注册用例的 Presenter。</summary>
public sealed class RegisterPresenter : IRegisterOutputPort
{
    public IActionResult? ViewModel { get; private set; }

    public void Error(string message)
    {
        ViewModel = new ConflictObjectResult(new ProblemDetails
        {
            Title = "Registration failed",
            Detail = message
        });
    }

    public void Standard(RegisterOutput output)
    {
        ViewModel = new CreatedResult(
            $"/api/auth/users/{output.UserId}",
            new { output.UserId, output.Email, output.DisplayName });
    }
}
