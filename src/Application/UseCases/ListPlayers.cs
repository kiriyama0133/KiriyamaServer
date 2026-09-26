using KiriyamaServer.Application.Boundaries.Rooms;
using KiriyamaServer.Application.Interfaces;
using KiriyamaServer.Application.Repositories;

namespace KiriyamaServer.Application.UseCases;

/// <summary>列出房间内的玩家（供客户端房间页面展示成员与做延迟探测）。</summary>
public sealed class ListPlayers(IRoomRepository roomRepository, IListPlayersOutputPort outputPort)
    : IUseCase<ListPlayersInput>
{
    private readonly IRoomRepository _roomRepository = roomRepository ?? throw new ArgumentNullException(nameof(roomRepository));
    private readonly IListPlayersOutputPort _outputPort = outputPort ?? throw new ArgumentNullException(nameof(outputPort));

    public async Task ExecuteAsync(ListPlayersInput input, CancellationToken cancellationToken = default)
    {
        var room = await _roomRepository.FindAsync(input.RoomId, cancellationToken);
        if (room is null)
        {
            _outputPort.NotFound();
            return;
        }

        var players = room.Players
            .Select(p => new PlayerInfo(p.Id, p.Nickname, p.NodeId, p.VirtualIp, room.IsHost(p.NodeId)))
            .ToList();

        _outputPort.Standard(new ListPlayersOutput(room.Id.Value, room.Name, room.HostNodeId, players));
    }
}
