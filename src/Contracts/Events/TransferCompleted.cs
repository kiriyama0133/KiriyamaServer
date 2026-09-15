using Genocs.Common.CQRS.Events;

namespace KiriyamaServer.Contracts.Events;

public sealed class TransferCompleted : IEvent
{
    public Guid OriginalAccountId { get; init; }
    public Guid DestinationAccountId { get; init; }
    public decimal Amount { get; init; }
}
