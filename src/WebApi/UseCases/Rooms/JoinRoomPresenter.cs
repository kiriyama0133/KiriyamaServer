using KiriyamaServer.Application.Boundaries.Rooms;
using Microsoft.AspNetCore.Mvc;

namespace KiriyamaServer.WebApi.UseCases.Rooms;

/// <summary>加入房间用例的 Presenter。</summary>
public sealed class JoinRoomPresenter : IJoinRoomOutputPort
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

    public void Unauthorized()
    {
        ViewModel = new UnauthorizedObjectResult(new ProblemDetails
        {
            Title = "Invalid password",
            Detail = "The room password is incorrect."
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

    public void Conflict()
    {
        ViewModel = new ConflictObjectResult(new ProblemDetails
        {
            Title = "Room full",
            Detail = "The room is full."
        });
    }

    public void Standard(JoinRoomOutput output)
    {
        ViewModel = new OkObjectResult(new JoinRoomResponse(output.PlayerId, output.RoomId));
    }
}
