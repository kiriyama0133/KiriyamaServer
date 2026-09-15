using KiriyamaServer.Application.Boundaries.Games;
using KiriyamaServer.Application.Interfaces;
using Microsoft.AspNetCore.Mvc;

namespace KiriyamaServer.WebApi.UseCases.Games;

/// <summary>
/// 游戏板块控制器。提供只读的游戏板块列表，供客户端按板块浏览房间。
/// </summary>
[Route("api/games")]
[ApiController]
public sealed class GamesController : ControllerBase
{
    private readonly IUseCase<ListGamesInput> _listGamesUseCase;
    private readonly ListGamesPresenter _listGamesPresenter;

    public GamesController(
        IUseCase<ListGamesInput> listGamesUseCase,
        ListGamesPresenter listGamesPresenter)
    {
        _listGamesUseCase = listGamesUseCase;
        _listGamesPresenter = listGamesPresenter;
    }

    /// <summary>列出所有可联机的游戏板块。</summary>
    [HttpGet]
    [ProducesResponseType(StatusCodes.Status200OK, Type = typeof(List<GameDto>))]
    [ProducesResponseType(StatusCodes.Status500InternalServerError)]
    public async Task<IActionResult?> ListAsync(CancellationToken cancellationToken = default)
    {
        await _listGamesUseCase.ExecuteAsync(ListGamesInput.Instance, cancellationToken);
        return _listGamesPresenter.ViewModel;
    }
}
