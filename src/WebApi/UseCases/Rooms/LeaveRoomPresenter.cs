using KiriyamaServer.Application.Boundaries.Rooms;
using Microsoft.AspNetCore.Mvc;

namespace KiriyamaServer.WebApi.UseCases.Rooms;

/// <summary>离开房间用例的 Presenter。</summary>
public sealed class LeaveRoomPresenter : ILeaveRoomOutputPort
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

    public void Standard(LeaveRoomOutput output)
    {
        ViewModel = new OkObjectResult(new { roomClosed = output.RoomClosed });
    }
}
