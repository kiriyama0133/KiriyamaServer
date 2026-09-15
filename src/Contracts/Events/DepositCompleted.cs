using Genocs.Common.CQRS.Events;

namespace KiriyamaServer.Contracts.Events;

public sealed class DepositCompleted : IEvent
{
    public Guid AccountId { get; init; }
    public decimal Amount { get; init; }
}
