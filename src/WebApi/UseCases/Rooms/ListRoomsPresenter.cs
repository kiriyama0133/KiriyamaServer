using KiriyamaServer.Application.Boundaries.Rooms;
using Microsoft.AspNetCore.Mvc;

namespace KiriyamaServer.WebApi.UseCases.Rooms;

/// <summary>列出房间用例的 Presenter。</summary>
public sealed class ListRoomsPresenter : IListRoomsOutputPort
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

    public void Standard(ListRoomsOutput output)
    {
        var rooms = output.Rooms
            .Select(r => new RoomDto(r.Id, r.Name, r.Host, r.GameKey, r.PlayerCount, r.MaxPlayers, r.HasPassword))
            .ToList();

        ViewModel = new OkObjectResult(rooms);
    }
}
