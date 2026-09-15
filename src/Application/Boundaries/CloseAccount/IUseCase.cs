namespace KiriyamaServer.Application.Boundaries.CloseAccount;

public interface IUseCase
{
    Task ExecuteAsync(CloseAccountInput closeAccountInput, CancellationToken cancellationToken = default);
}