using KiriyamaServer.Application.Boundaries.Rooms;
using Microsoft.AspNetCore.Mvc;

namespace KiriyamaServer.WebApi.UseCases.Rooms;

/// <summary>列出房间内玩家用例的 Presenter。</summary>
public sealed class ListPlayersPresenter : IListPlayersOutputPort
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

    public void Standard(ListPlayersOutput output)
    {
        var players = output.Players
            .Select(p => new PlayerDto(p.PlayerId, p.Nickname, p.NodeId, p.VirtualIp))
            .ToList();

        ViewModel = new OkObjectResult(new
        {
            roomId = output.RoomId,
            roomName = output.RoomName,
            players
        });
    }
}
