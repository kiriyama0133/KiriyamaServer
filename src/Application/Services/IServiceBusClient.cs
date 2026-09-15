using Genocs.Common.CQRS.Commands;
using Genocs.Common.CQRS.Events;

namespace KiriyamaServer.Application.Services;

public interface IServiceBusClient
{
    Task SendCommandAsync<T>(T command, CancellationToken cancellationToken = default)
        where T : ICommand;

    Task PublishEventAsync<T>(T message, CancellationToken cancellationToken = default)
        where T : IEvent;
}