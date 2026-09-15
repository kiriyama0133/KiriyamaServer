using KiriyamaServer.Application.Boundaries.Games;
using KiriyamaServer.Application.Interfaces;
using KiriyamaServer.Application.Repositories;

namespace KiriyamaServer.Application.UseCases;

/// <summary>列出所有可联机的游戏板块。</summary>
public sealed class ListGames(IGameCatalog gameCatalog, IListGamesOutputPort outputPort)
    : IUseCase<ListGamesInput>
{
    private readonly IGameCatalog _gameCatalog = gameCatalog ?? throw new ArgumentNullException(nameof(gameCatalog));
    private readonly IListGamesOutputPort _outputPort = outputPort ?? throw new ArgumentNullException(nameof(outputPort));

    public async Task ExecuteAsync(ListGamesInput input, CancellationToken cancellationToken = default)
    {
        var games = await _gameCatalog.ListAsync(cancellationToken);

        var infos = games
            .Select(g => new GameInfo(g.Key.Value, g.DisplayName))
            .ToList();

        _outputPort.Standard(new ListGamesOutput(infos));
    }
}
