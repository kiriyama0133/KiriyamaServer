using Genocs.Common.CQRS.Events;

namespace KiriyamaServer.Contracts.Events;

public sealed class WithdrawCompleted : IEvent
{
    public Guid AccountId { get; init; }
    public decimal Amount { get; init; }
}
