using KiriyamaServer.Application.Boundaries.Rooms;
using Microsoft.AspNetCore.Mvc;

namespace KiriyamaServer.WebApi.UseCases.Rooms;

/// <summary>转让房主用例的 Presenter。</summary>
public sealed class TransferHostPresenter : ITransferHostOutputPort
{
    public IActionResult? ViewModel { get; private set; }

    public void Error(string message)
    {
        ViewModel = new BadRequestObjectResult(new ProblemDetails
        {
            Title = "An error occurred",
            Detail = message
        });
    }

    public void NotFound()
    {
        ViewModel = new NotFoundObjectResult(new ProblemDetails
        {
            Title = "Room not found",
            Detail = "The room does not exist or has been closed."
        });
    }

    public void Forbidden(string message)
    {
        ViewModel = new ObjectResult(new ProblemDetails
        {
            Title = "Forbidden",
            Detail = message,
            Status = StatusCodes.Status403Forbidden
        })
        {
            StatusCode = StatusCodes.Status403Forbidden
        };
    }

    public void Standard(TransferHostOutput output)
    {
        ViewModel = new OkObjectResult(new
        {
            roomId = output.RoomId,
            hostName = output.HostName,
            hostNodeId = output.HostNodeId
        });
    }
}
