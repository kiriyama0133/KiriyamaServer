using KiriyamaServer.Application.Interfaces;

namespace KiriyamaServer.Application.Boundaries.Games;

/// <summary>列出游戏板块用例的输出端口。</summary>
public interface IListGamesOutputPort : IErrorHandler
{
    void Standard(ListGamesOutput output);
}
