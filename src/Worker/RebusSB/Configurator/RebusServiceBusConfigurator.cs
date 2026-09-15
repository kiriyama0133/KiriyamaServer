using KiriyamaServer.Infrastructure.RebusSB;
using KiriyamaServer.Worker.RebusSB.HostedServices;

namespace KiriyamaServer.Worker.Configurator;

public static class RebusServiceBusConfigurator
{
    public static IServiceCollection ConfigureRebus(this IServiceCollection services, IConfiguration configuration)
    {
        services.Configure<RebusBusSettings>(configuration.GetSection(RebusBusSettings.Position));
        services.AddHostedService<RebusHostedService>();
        return services;
    }
}
