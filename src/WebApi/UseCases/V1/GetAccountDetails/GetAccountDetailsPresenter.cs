using KiriyamaServer.Application.Boundaries.GetAccountDetails;
using KiriyamaServer.WebApi.ViewModels;
using Microsoft.AspNetCore.Mvc;

namespace KiriyamaServer.WebApi.UseCases.V1.GetAccountDetails;

public sealed class GetAccountDetailsPresenter : IOutputPort
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

    public void Default(GetAccountDetailsOutput getAccountDetailsOutput)
    {
        List<TransactionModel> transactions = new List<TransactionModel>();

        foreach (var item in getAccountDetailsOutput.Transactions)
        {
            var transaction = new TransactionModel(
                item.Amount,
                item.Description,
                item.TransactionDate);

            transactions.Add(transaction);
        }

        var getAccountDetailsResponse = new GetAccountDetailsResponse(
            getAccountDetailsOutput.AccountId,
            getAccountDetailsOutput.CurrentBalance,
            transactions);

        ViewModel = new OkObjectResult(getAccountDetailsResponse);
    }

    public void NotFound(string message)
    {
        ViewModel = new NotFoundObjectResult(message);
    }
}