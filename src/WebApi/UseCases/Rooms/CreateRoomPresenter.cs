using KiriyamaServer.Application.Boundaries.Rooms;
using Microsoft.AspNetCore.Mvc;

namespace KiriyamaServer.WebApi.UseCases.Rooms;

/// <summary>创建房间用例的 Presenter。</summary>
public sealed class CreateRoomPresenter : ICreateRoomOutputPort
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

    public void Standard(CreateRoomOutput output)
    {
        var room = output.Room;
        ViewModel = new OkObjectResult(new RoomDto(room.Id, room.Name, room.Host, room.GameKey, room.PlayerCount, room.MaxPlayers, room.HasPassword));
    }
}
