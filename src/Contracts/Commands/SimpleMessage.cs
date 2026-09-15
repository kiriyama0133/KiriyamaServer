using Genocs.Common.CQRS.Commands;

namespace KiriyamaServer.Contracts.Commands;

public class SimpleMessage : ICommand
{
    public required string MessageId { get; init; }

    public required string MessageBody { get; init; }
}