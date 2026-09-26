using KiriyamaServer.Application.Boundaries.Rooms;
using KiriyamaServer.Application.Interfaces;
using KiriyamaServer.Application.Repositories;
using KiriyamaServer.Application.Services;
using KiriyamaServer.Domain;
using KiriyamaServer.Domain.Rooms;

namespace KiriyamaServer.Application.UseCases;

/// <summary>
/// 转让房主：仅当前房主可发起，接手者必须已在房间内。
///
/// 转让成功后向房间所有订阅者广播 <see cref="RoomEventTypes.HostChanged"/>，
/// 房内客户端据此即时刷新房主标识与转让按钮的可见性。
/// </summary>
public sealed class TransferHost(
    IRoomRepository roomRepository,
    IRoomEventPublisher eventPublisher,
    ITransferHostOutputPort outputPort)
    : IUseCase<TransferHostInput>
{
    private readonly IRoomRepository _roomRepository = roomRepository ?? throw new ArgumentNullException(nameof(roomRepository));
    private readonly IRoomEventPublisher _eventPublisher = eventPublisher ?? throw new ArgumentNullException(nameof(eventPublisher));
    private readonly ITransferHostOutputPort _outputPort = outputPort ?? throw new ArgumentNullException(nameof(outputPort));

    public async Task ExecuteAsync(TransferHostInput input, CancellationToken cancellationToken = default)
    {
        var room = await _roomRepository.FindAsync(input.RoomId, cancellationToken);
        if (room is null)
        {
            _outputPort.NotFound();
            return;
        }

        // 权限前置校验放到输出端口上，返回 403 而不是笼统的领域异常。
        if (!room.IsHost(input.RequesterNodeId))
        {
            _outputPort.Forbidden("只有房主可以转让房主身份。");
            return;
        }

        Player target;

        try
        {
            target = room.TransferHost(input.RequesterNodeId, input.TargetNodeId);
        }
        catch (DomainException ex)
        {
            // 目标不在房间内 / 目标本来就是房主：转成可读的 400。
            _outputPort.Error(ex.Message);
            return;
        }

        // 广播房主变更（先广播再返回，让被转让者尽快刷新界面）。
        _eventPublisher.PublishHostChanged(room.Id.Value, target.Nickname, target.NodeId);

        _outputPort.Standard(new TransferHostOutput(room.Id.Value, target.Nickname, target.NodeId));
    }
}
