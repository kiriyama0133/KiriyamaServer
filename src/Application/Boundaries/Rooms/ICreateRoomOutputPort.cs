using KiriyamaServer.Application.Interfaces;

namespace KiriyamaServer.Application.Boundaries.Rooms;

/// <summary>创建房间用例的输出端口。</summary>
public interface ICreateRoomOutputPort : IErrorHandler
{
    void Standard(CreateRoomOutput output);
}
