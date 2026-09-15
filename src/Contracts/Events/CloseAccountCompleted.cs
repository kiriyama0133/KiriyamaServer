using Genocs.Common.CQRS.Events;

namespace KiriyamaServer.Contracts.Events;

public sealed class CloseAccountCompleted : IEvent
{
    public Guid AccountId { get; init; }
}
