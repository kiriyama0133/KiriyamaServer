using KiriyamaServer.Application.Interfaces;

namespace KiriyamaServer.Application.Boundaries.Rooms;

/// <summary>列出房间用例的输出端口。</summary>
public interface IListRoomsOutputPort : IErrorHandler
{
    void Standard(ListRoomsOutput output);
}
