using KiriyamaServer.Application.Interfaces;

namespace KiriyamaServer.Application.Boundaries.Rooms;

/// <summary>离开房间用例的输出端口。</summary>
public interface ILeaveRoomOutputPort : IErrorHandler
{
    void Standard(LeaveRoomOutput output);

    /// <summary>房间不存在。</summary>
    void NotFound();
}
