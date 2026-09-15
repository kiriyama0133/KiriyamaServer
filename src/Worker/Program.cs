using KiriyamaServer.Worker.Configurator;
using KiriyamaServer.Worker.HealthCheck;
using KiriyamaServer.Worker.WebApi;
using Genocs.Core.Builders;
using Genocs.Logging;
using Genocs.Telemetry;
using Serilog;

StaticLogger.EnsureInitialized();

IGenocsBuilder? gnxBuilder = null;

IHost host = Host.CreateDefaultBuilder(args)
    .UseLogging()
    .ConfigureServices((hostContext, services) =>
    {
        gnxBuilder = services
            .AddGenocs(hostContext.Configuration)
            .AddTelemetry();

        services.ConfigureRebus(hostContext.Configuration);

        // Add other services here
        services.ConfigureWebApiServices(hostContext.Configuration);

        // Add health checks
        services.ConfigureHealthChecks(hostContext.Configuration);
    })
    .Build();

gnxBuilder?.Build(host.Services);

await host.RunAsync();

await Log.CloseAndFlushAsync();
