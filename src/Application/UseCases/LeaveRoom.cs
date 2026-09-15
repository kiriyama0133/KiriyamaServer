using KiriyamaServer.Application.Boundaries.Rooms;
using KiriyamaServer.Application.Interfaces;
using KiriyamaServer.Application.Repositories;
using KiriyamaServer.Application.Services;
using KiriyamaServer.Domain.Rooms;

namespace KiriyamaServer.Application.UseCases;

/// <summary>
/// 离开房间：按节点定位玩家、移除，并通过 Controller 清除该节点的房间 Tag。
/// 空房间自动解散。
/// </summary>
public sealed class LeaveRoom(IRoomRepository roomRepository, IZeroTierController zeroTierController, ILeaveRoomOutputPort outputPort)
    : IUseCase<LeaveRoomInput>
{
    private readonly IRoomRepository _roomRepository = roomRepository ?? throw new ArgumentNullException(nameof(roomRepository));
    private readonly IZeroTierController _zeroTierController = zeroTierController ?? throw new ArgumentNullException(nameof(zeroTierController));
    private readonly ILeaveRoomOutputPort _outputPort = outputPort ?? throw new ArgumentNullException(nameof(outputPort));

    public async Task ExecuteAsync(LeaveRoomInput input, CancellationToken cancellationToken = default)
    {
        var room = await _roomRepository.FindAsync(input.RoomId, cancellationToken);
        if (room is null)
        {
            _outputPort.NotFound();
            return;
        }

        Player? player = room.Players.FirstOrDefault(p => string.Equals(p.NodeId, input.NodeId, StringComparison.OrdinalIgnoreCase));
        if (player is null)
        {
            _outputPort.NotFound();
            return;
        }

        room.Leave(player.Id);

        bool roomClosed = room.IsEmpty;
        if (roomClosed)
        {
            await _roomRepository.RemoveAsync(room.Id, cancellationToken);
        }

        // 清除该节点的房间 Tag，使其退出房间的流量范围。
        await _zeroTierController.ClearRoomTagAsync(player.NodeId, cancellationToken);

        _outputPort.Standard(new LeaveRoomOutput(roomClosed));
    }
}
