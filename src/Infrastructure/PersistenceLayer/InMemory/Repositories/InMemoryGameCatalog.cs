using KiriyamaServer.Application.Repositories;
using KiriyamaServer.Domain.Games;

namespace KiriyamaServer.Infrastructure.PersistenceLayer.InMemory.Repositories;

/// <summary>
/// 内存游戏目录。内置一份只读的游戏板块列表，
/// 供客户端按板块浏览房间。
/// </summary>
public sealed class InMemoryGameCatalog : IGameCatalog
{
    // 内置的游戏板块（只读列表）。后续可改为从配置/数据库加载。
    private static readonly IReadOnlyList<Game> BuiltInGames = new List<Game>
    {
        new(GameKey.From("civ6"), "文明 6"),
        new(GameKey.From("mc"), "Minecraft"),
    };

    public Task<IReadOnlyList<Game>> ListAsync(CancellationToken cancellationToken = default)
        => Task.FromResult(BuiltInGames);

    public Task<Game?> FindAsync(GameKey key, CancellationToken cancellationToken = default)
        => Task.FromResult(BuiltInGames.FirstOrDefault(g => g.Key == key));
}
