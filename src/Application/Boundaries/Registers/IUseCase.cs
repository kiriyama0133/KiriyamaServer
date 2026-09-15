namespace KiriyamaServer.Application.Boundaries.Registers;

public interface IUseCase
{
    Task ExecuteAsync(RegisterInput registerInput, CancellationToken cancellationToken = default);
}