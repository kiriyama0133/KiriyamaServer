using KiriyamaServer.Application.Services;
using KiriyamaServer.Infrastructure.RebusSB;

namespace KiriyamaServer.WebApi.Extensions;

public static class RebusServicebusInfrastructureExtensions
{
    public static IServiceCollection AddRebusServiceBus(this IServiceCollection services, IConfiguration config)
    {
        services.Configure<RebusBusSettings>(config.GetSection(RebusBusSettings.Position));

        services.AddSingleton<IServiceBusClient, RebusServiceBusClient>();

        return services;
    }
}
