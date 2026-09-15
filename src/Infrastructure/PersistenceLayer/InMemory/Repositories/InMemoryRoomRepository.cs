using KiriyamaServer.Application.Repositories;
using KiriyamaServer.Domain.Games;
using KiriyamaServer.Domain.Rooms;
using Microsoft.Extensions.Logging;

namespace KiriyamaServer.Infrastructure.PersistenceLayer.InMemory.Repositories;

/// <summary>
/// 内存房间仓库。游戏房间是运行时临时状态，随进程生命周期存在，
/// 用线程安全字典 + 读写锁维护，无需持久化到数据库。
/// </summary>
public sealed class InMemoryRoomRepository(ILogger<InMemoryRoomRepository> logger) : IRoomRepository
{
    private readonly ILogger<InMemoryRoomRepository> _logger = logger ?? throw new ArgumentNullException(nameof(logger));
    private readonly Dictionary<string, GameRoom> _rooms = new();
    private readonly Lock _lock = new();

    public Task<IReadOnlyList<GameRoom>> ListAsync(CancellationToken cancellationToken = default)
    {
        lock (_lock)
        {
            return Task.FromResult<IReadOnlyList<GameRoom>>(_rooms.Values.ToList());
        }
    }

    public Task<IReadOnlyList<GameRoom>> ListByGameAsync(GameKey? gameKey, CancellationToken cancellationToken = default)
    {
        lock (_lock)
        {
            IReadOnlyList<GameRoom> rooms = gameKey is null
                ? _rooms.Values.ToList()
                : _rooms.Values.Where(r => r.GameKey == gameKey).ToList();

            return Task.FromResult(rooms);
        }
    }

    public Task<GameRoom?> FindAsync(RoomId roomId, CancellationToken cancellationToken = default)
    {
        lock (_lock)
        {
            _rooms.TryGetValue(roomId.Value, out GameRoom? room);
            return Task.FromResult(room);
        }
    }

    public Task<GameRoom?> FindByNodeIdAsync(string nodeId, CancellationToken cancellationToken = default)
    {
        lock (_lock)
        {
            GameRoom? room = _rooms.Values.FirstOrDefault(r =>
                r.Players.Any(p => string.Equals(p.NodeId, nodeId, StringComparison.OrdinalIgnoreCase)));
            return Task.FromResult(room);
        }
    }

    public Task AddAsync(GameRoom room, CancellationToken cancellationToken = default)
    {
        lock (_lock)
        {
            _rooms[room.Id.Value] = room;
        }

        _logger.LogInformation("房间已创建：{RoomId}（{Name}，房主 {Host}，Tag {Tag}）", room.Id, room.Name, room.HostName, room.RoomTag.Value);
        return Task.CompletedTask;
    }

    public Task RemoveAsync(RoomId roomId, CancellationToken cancellationToken = default)
    {
        lock (_lock)
        {
            _rooms.Remove(roomId.Value);
        }

        _logger.LogInformation("房间已关闭：{RoomId}", roomId);
        return Task.CompletedTask;
    }
}
