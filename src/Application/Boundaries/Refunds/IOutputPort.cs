using KiriyamaServer.Application.Interfaces;

namespace KiriyamaServer.Application.Boundaries.Refunds;

public interface IOutputPort : IErrorHandler
{
    void Default(RefundOutput refundOutput);
}