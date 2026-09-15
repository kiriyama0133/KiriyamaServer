using KiriyamaServer.Application.Boundaries.CloseAccount;
using KiriyamaServer.Application.Interfaces;
using Microsoft.AspNetCore.Mvc;

namespace KiriyamaServer.WebApi.UseCases.V1.CloseAccount;

public sealed class CloseAccountPresenter : IOutputPort<CloseAccountOutput>
{
    public IActionResult? ViewModel { get; private set; }

    public void Error(string message)
    {
        var problemDetails = new ProblemDetails()
        {
            Title = "An error occurred",
            Detail = message
        };

        ViewModel = new BadRequestObjectResult(problemDetails);
    }

    public void Default(CloseAccountOutput closeAccountOutput)
    {
        ViewModel = new OkResult();
    }
}