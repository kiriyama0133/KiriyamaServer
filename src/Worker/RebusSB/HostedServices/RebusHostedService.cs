using KiriyamaServer.Contracts.Events;
using KiriyamaServer.Infrastructure.RebusSB;
using KiriyamaServer.Worker.RebusSB.Handlers;
using Microsoft.Extensions.Options;
using Rebus.Activation;
using Rebus.Bus;
using Rebus.Config;

namespace KiriyamaServer.Worker.RebusSB.HostedServices;

internal class RebusHostedService(IOptions<RebusBusSettings> settings, ILogger<RebusHostedService> logger) : IHostedService
{
    private readonly ILogger<RebusHostedService> _logger = logger ?? throw new ArgumentNullException(nameof(logger));
    private readonly RebusBusSettings _settings = settings.Value ?? throw new NullReferenceException("options cannot be null");

    private BuiltinHandlerActivator? _activator;
    private IBus? _bus;

    public async Task StartAsync(CancellationToken cancellationToken)
    {
        _logger.LogInformation("Starting...");

        // Start rebus configuration
        _activator = new BuiltinHandlerActivator();

        _activator.Register(() => new RegistrationCompletedHandler(_logger));

        _bus = Configure.With(_activator)
            .Logging(l => l.ColoredConsole(minLevel: Rebus.Logging.LogLevel.Debug))
            .Transport(t => t.UseRabbitMq(_settings.TransportConnection, _settings.QueueName))
            .Options(o => o.SetMaxParallelism(1))
            .Start();

        // Subscribe the event
        await _activator.Bus.Subscribe<RegistrationCompleted>();

        _logger.LogInformation("Started");

    }

    public async Task StopAsync(CancellationToken cancellationToken)
    {
        _logger.LogInformation("Stopping...");
        _logger.LogInformation("Stopped");
        await Task.CompletedTask;
    }
}
