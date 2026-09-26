using KiriyamaServer.Application.Boundaries.Rooms;
using KiriyamaServer.Application.Interfaces;
using KiriyamaServer.Application.Repositories;
using KiriyamaServer.Application.Services;
using KiriyamaServer.Domain.Rooms;

namespace KiriyamaServer.Application.UseCases;

/// <summary>
/// 离开房间：按节点定位玩家、移除，并通过 Controller 清除该节点的房间 Tag。
///
/// 房间销毁有两种情形：
///   1. 房主退出 —— 房主是房间的锚点，他一走整个房间就解散（房内其余成员一并被请出）；
///   2. 房间空了 —— 最后一名成员离开后自动解散。
/// 两种情形都会通过 <see cref="IRoomEventPublisher"/> 广播 room_closed，
/// 让仍连着 SSE 的客户端立即退出房间页面，而不是等下一次轮询。
/// </summary>
public sealed class LeaveRoom(
    IRoomRepository roomRepository,
    IZeroTierController zeroTierController,
    IRoomEventPublisher eventPublisher,
    ILeaveRoomOutputPort outputPort)
    : IUseCase<LeaveRoomInput>
{
    private readonly IRoomRepository _roomRepository = roomRepository ?? throw new ArgumentNullException(nameof(roomRepository));
    private readonly IZeroTierController _zeroTierController = zeroTierController ?? throw new ArgumentNullException(nameof(zeroTierController));
    private readonly IRoomEventPublisher _eventPublisher = eventPublisher ?? throw new ArgumentNullException(nameof(eventPublisher));
    private readonly ILeaveRoomOutputPort _outputPort = outputPort ?? throw new ArgumentNullException(nameof(outputPort));

    public async Task ExecuteAsync(LeaveRoomInput input, CancellationToken cancellationToken = default)
    {
        var room = await _roomRepository.FindAsync(input.RoomId, cancellationToken);
        if (room is null)
        {
            _outputPort.NotFound();
            return;
        }

        // 先判断身份，再把人移出去（移除后 IsHost 就查不到了）。
        bool leftByHost = room.IsHost(input.NodeId);

        // 按节点移除该玩家的全部成员记录（历史重复加入可能遗留多条，一次清光）。
        int removed = room.LeaveByNodeId(input.NodeId);
        if (removed == 0)
        {
            _outputPort.NotFound();
            return;
        }

        bool roomClosed = leftByHost || room.IsEmpty;

        if (roomClosed)
        {
            // 房间要销毁：把仍在房内的成员（房主退出时剩下的那些人）的 Tag 一并清掉，
            // 否则他们会留在 ZeroTier 的流量范围内却已经没有房间了。
            foreach (Player remaining in room.Players.ToList())
            {
                await _zeroTierController.ClearRoomTagAsync(remaining.NodeId, cancellationToken);
            }

            await _roomRepository.RemoveAsync(room.Id, cancellationToken);
        }

        // 清除离开者自己的房间 Tag，使其退出房间的流量范围。
        await _zeroTierController.ClearRoomTagAsync(input.NodeId, cancellationToken);

        if (roomClosed)
        {
            // 广播放在最后：此时房间已从仓库移除，客户端收到事件后再查房间会得到 404。
            _eventPublisher.PublishRoomClosed(
                room.Id.Value,
                leftByHost ? "房主已退出，房间已解散。" : "房间已解散。");
        }

        _outputPort.Standard(new LeaveRoomOutput(roomClosed));
    }
}
