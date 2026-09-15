using KiriyamaServer.Application.Boundaries.Refunds;

namespace KiriyamaServer.Infrastructure.PersistenceLayer.Presenters;

public sealed class RefundPresenter : IOutputPort
{
    public List<string> Errors { get; }
    public List<RefundOutput> Refunds { get; }

    public RefundPresenter()
    {
        Errors = [];
        Refunds = [];
    }

    public void Error(string message)
        => Errors.Add(message);

    public void Default(RefundOutput output)
        => Refunds.Add(output);
}