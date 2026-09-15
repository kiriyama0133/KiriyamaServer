using KiriyamaServer.Domain.Games;
using KiriyamaServer.Domain.Rooms;

namespace KiriyamaServer.Application.Repositories;

/// <summary>
/// 房间仓库（驱动端口）。负责房间与玩家连接状态的存储与检索。
/// </summary>
public interface IRoomRepository
{
    /// <summary>列出所有存活的房间。</summary>
    Task<IReadOnlyList<GameRoom>> ListAsync(CancellationToken cancellationToken = default);

    /// <summary>按游戏板块列出房间；gameKey 为 null 时列出全部。</summary>
    Task<IReadOnlyList<GameRoom>> ListByGameAsync(GameKey? gameKey, CancellationToken cancellationToken = default);

    /// <summary>按房间标识查找房间。</summary>
    Task<GameRoom?> FindAsync(RoomId roomId, CancellationToken cancellationToken = default);

    /// <summary>按玩家节点 ID 查找其所在房间（用于离开时定位房间）。</summary>
    Task<GameRoom?> FindByNodeIdAsync(string nodeId, CancellationToken cancellationToken = default);

    /// <summary>新增一个房间。</summary>
    Task AddAsync(GameRoom room, CancellationToken cancellationToken = default);

    /// <summary>移除一个房间（房主解散或房间空置）。</summary>
    Task RemoveAsync(RoomId roomId, CancellationToken cancellationToken = default);
}
