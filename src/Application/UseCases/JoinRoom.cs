using KiriyamaServer.Application.Boundaries.Rooms;
using KiriyamaServer.Application.Interfaces;
using KiriyamaServer.Application.Repositories;
using KiriyamaServer.Application.Services;

namespace KiriyamaServer.Application.UseCases;

/// <summary>
/// 加入房间：密码校验 + 人数上限 + 通过 Controller 给节点打房间 Tag。
/// 打上 Tag 后，ZeroTier 网络 Flow Rules 即放行该节点与同房间节点互通。
/// </summary>
public sealed class JoinRoom(IRoomRepository roomRepository, IZeroTierController zeroTierController, IJoinRoomOutputPort outputPort)
    : IUseCase<JoinRoomInput>
{
    private readonly IRoomRepository _roomRepository = roomRepository ?? throw new ArgumentNullException(nameof(roomRepository));
    private readonly IZeroTierController _zeroTierController = zeroTierController ?? throw new ArgumentNullException(nameof(zeroTierController));
    private readonly IJoinRoomOutputPort _outputPort = outputPort ?? throw new ArgumentNullException(nameof(outputPort));

    public async Task ExecuteAsync(JoinRoomInput input, CancellationToken cancellationToken = default)
    {
        var room = await _roomRepository.FindAsync(input.RoomId, cancellationToken);
        if (room is null)
        {
            _outputPort.NotFound();
            return;
        }

        if (!room.IsPasswordValid(input.Password))
        {
            _outputPort.Unauthorized();
            return;
        }

        if (room.IsFull)
        {
            _outputPort.Conflict();
            return;
        }

        var player = room.Join(input.Nickname, input.NodeId, input.VirtualIp);

        // 通过 Controller 给该节点打上房间 Tag，实现房间级网络隔离。
        await _zeroTierController.AssignRoomTagAsync(player.NodeId, room.RoomTag.Value, cancellationToken);

        _outputPort.Standard(new JoinRoomOutput(player.Id, room.Id.Value));
    }
}
