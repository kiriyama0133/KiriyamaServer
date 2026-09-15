using KiriyamaServer.Application.Interfaces;

namespace KiriyamaServer.Application.Boundaries.CloseAccount;

public interface IOutputPort : IErrorHandler
{
    void Default(CloseAccountOutput closeAccountOutput);
}