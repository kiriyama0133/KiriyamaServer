using KiriyamaServer.Application.Boundaries.Games;
using Microsoft.AspNetCore.Mvc;

namespace KiriyamaServer.WebApi.UseCases.Games;

/// <summary>列出游戏板块用例的 Presenter。</summary>
public sealed class ListGamesPresenter : IListGamesOutputPort
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

    public void Standard(ListGamesOutput output)
    {
        var games = output.Games
            .Select(g => new GameDto(g.Key, g.DisplayName))
            .ToList();

        ViewModel = new OkObjectResult(games);
    }
}
