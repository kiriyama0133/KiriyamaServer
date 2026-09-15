namespace KiriyamaServer.Application.Boundaries.Refunds;

public interface IUseCase
{
    Task ExecuteAsync(RefundInput refundInput, CancellationToken cancellationToken = default);
}