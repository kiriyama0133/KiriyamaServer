using Genocs.Common.CQRS.Events;

namespace KiriyamaServer.Contracts.Events;

public class DemoEventOccurred : IIntegrationEvent
{
    public string? Payload { get; init; }
    public int Value { get; init; }
}
