using KiriyamaServer.Application.Boundaries.Rooms;
using KiriyamaServer.Application.Interfaces;
using KiriyamaServer.Application.Repositories;

namespace KiriyamaServer.Application.UseCases;

/// <summary>列出所有房间。</summary>
public sealed class ListRooms(IRoomRepository roomRepository, IListRoomsOutputPort outputPort)
    : IUseCase<ListRoomsInput>
{
    private readonly IRoomRepository _roomRepository = roomRepository ?? throw new ArgumentNullException(nameof(roomRepository));
    private readonly IListRoomsOutputPort _outputPort = outputPort ?? throw new ArgumentNullException(nameof(outputPort));

    public async Task ExecuteAsync(ListRoomsInput input, CancellationToken cancellationToken = default)
    {
        var rooms = await _roomRepository.ListByGameAsync(input.GameKey, cancellationToken);

        var infos = rooms
            .Select(r => new RoomInfo(
                r.Id.Value,
                r.Name,
                r.HostName,
                r.GameKey.Value,
                r.PlayerCount,
                r.MaxPlayers,
                r.HasPassword))
            .ToList();

        _outputPort.Standard(new ListRoomsOutput(infos));
    }
}
