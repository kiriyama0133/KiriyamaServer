using Genocs.Common.CQRS.Events;

namespace KiriyamaServer.Contracts.Events;

public class IntegrationEventIssued : IIntegrationEvent
{
    public string? Title { get; init; }
}
