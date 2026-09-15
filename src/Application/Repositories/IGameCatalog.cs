using KiriyamaServer.Domain.Games;

namespace KiriyamaServer.Application.Repositories;

/// <summary>
/// 游戏目录（驱动端口）。提供只读的游戏板块列表，
/// 供客户端按板块浏览房间。
/// </summary>
public interface IGameCatalog
{
    /// <summary>列出所有可联机的游戏板块。</summary>
    Task<IReadOnlyList<Game>> ListAsync(CancellationToken cancellationToken = default);

    /// <summary>按游戏标识查找游戏；不存在返回 null。</summary>
    Task<Game?> FindAsync(GameKey key, CancellationToken cancellationToken = default);
}
