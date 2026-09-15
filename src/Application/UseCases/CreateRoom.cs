using KiriyamaServer.Application.Boundaries.Rooms;
using KiriyamaServer.Application.Interfaces;
using KiriyamaServer.Application.Repositories;
using KiriyamaServer.Domain.Rooms;

namespace KiriyamaServer.Application.UseCases;

/// <summary>创建房间（生成房间标识与对应的 ZeroTier Tag 值）。</summary>
public sealed class CreateRoom(IRoomRepository roomRepository, ICreateRoomOutputPort outputPort)
    : IUseCase<CreateRoomInput>
{
    private readonly IRoomRepository _roomRepository = roomRepository ?? throw new ArgumentNullException(nameof(roomRepository));
    private readonly ICreateRoomOutputPort _outputPort = outputPort ?? throw new ArgumentNullException(nameof(outputPort));

    public async Task ExecuteAsync(CreateRoomInput input, CancellationToken cancellationToken = default)
    {
        // 生成唯一房间号（碰撞则重试）。
        RoomId roomId;
        do
        {
            roomId = RoomId.New();
        }
        while (await _roomRepository.FindAsync(roomId, cancellationToken) is not null);

        // 为房间生成唯一的 ZeroTier Tag 值，记录到房间数据。
        RoomTag roomTag = RoomTag.New();

        RoomPassword? password = input.Password is null
            ? null
            : RoomPassword.FromPlainText(input.Password);

        var room = new GameRoom(roomId, input.Name, input.HostName, input.GameKey, roomTag, password, input.MaxPlayers);

        await _roomRepository.AddAsync(room, cancellationToken);

        _outputPort.Standard(new CreateRoomOutput(
            new RoomInfo(room.Id.Value, room.Name, room.HostName, room.GameKey.Value, room.PlayerCount, room.MaxPlayers, room.HasPassword)));
    }
}
