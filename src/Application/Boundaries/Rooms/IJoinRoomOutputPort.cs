using KiriyamaServer.Application.Interfaces;

namespace KiriyamaServer.Application.Boundaries.Rooms;

/// <summary>加入房间用例的输出端口。</summary>
public interface IJoinRoomOutputPort : IErrorHandler
{
    void Standard(JoinRoomOutput output);

    /// <summary>密码错误。</summary>
    void Unauthorized();

    /// <summary>房间不存在。</summary>
    void NotFound();

    /// <summary>房间已满。</summary>
    void Conflict();
}
