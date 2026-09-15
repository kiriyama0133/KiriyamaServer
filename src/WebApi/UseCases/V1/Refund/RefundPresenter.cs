using KiriyamaServer.Application.Boundaries.Refunds;
using Microsoft.AspNetCore.Mvc;

namespace KiriyamaServer.WebApi.UseCases.V1.Refund;

public sealed class RefundPresenter : IOutputPort
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

    public void Default(RefundOutput withdrawOutput)
    {
        var withdrawResponse = new RefundResponse(
                                                    withdrawOutput.Transaction.Amount,
                                                    withdrawOutput.Transaction.Description,
                                                    withdrawOutput.Transaction.TransactionDate,
                                                    withdrawOutput.UpdatedBalance);

        ViewModel = new ObjectResult(withdrawResponse);
    }
}