using KiriyamaServer.Application.Boundaries.Games;
using KiriyamaServer.Application.Boundaries.Rooms;
using KiriyamaServer.Application.Interfaces;
using KiriyamaServer.Application.Repositories;
using KiriyamaServer.Application.Services;
using KiriyamaServer.Application.UseCases;
using KiriyamaServer.Infrastructure.Networking;
using KiriyamaServer.Infrastructure.PersistenceLayer.InMemory.Repositories;
using KiriyamaServer.WebApi.UseCases.Games;
using KiriyamaServer.WebApi.UseCases.Rooms;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;

namespace KiriyamaServer.WebApi.Extensions;

/// <summary>房间大厅与 ZeroTier Controller 的依赖注入装配。</summary>
public static class RoomsExtensions
{
    public static IServiceCollection AddRooms(this IServiceCollection services, IConfiguration configuration)
    {
        // 仓储（内存实现：游戏房间是运行时临时状态）。
        services.AddSingleton<IRoomRepository, InMemoryRoomRepository>();

        // 游戏目录（只读的游戏板块列表）。
        services.AddSingleton<IGameCatalog, InMemoryGameCatalog>();

        // ZeroTier Controller 配置。
        services.Configure<ZeroTierControllerOptions>(configuration.GetSection("ZeroTier"));

        // 平台适配（运行时判定 Windows/Linux，定位 authtoken 与控制端口）。
        services.AddSingleton<IZeroTierControllerPlatform, RuntimeZeroTierControllerPlatform>();

        // Controller 后端按配置切换：SelfHosted（本机控制端口）或 Official（my.zerotier.com）。
        // 两个实现内部都用 IHttpClientFactory.CreateClient(名字) 自建客户端（不采用 Typed HttpClient），
        // 所以这里注册具名 HttpClient + 单例服务类即可（不能用 AddHttpClient<T>，其构造函数不含 HttpClient 参数）。
        services.AddHttpClient(nameof(SelfHostedZeroTierController));
        services.AddHttpClient(nameof(OfficialApiZeroTierController));
        services.AddSingleton<SelfHostedZeroTierController>();
        services.AddSingleton<OfficialApiZeroTierController>();
        services.AddSingleton<IZeroTierController>(sp =>
        {
            var options = sp.GetRequiredService<Microsoft.Extensions.Options.IOptions<ZeroTierControllerOptions>>().Value;
            return string.Equals(options.Backend, "Official", StringComparison.OrdinalIgnoreCase)
                ? sp.GetRequiredService<OfficialApiZeroTierController>()
                : sp.GetRequiredService<SelfHostedZeroTierController>();
        });

        // 用例。
        services.AddScoped<IUseCase<ListRoomsInput>, ListRooms>();
        services.AddScoped<IUseCase<CreateRoomInput>, CreateRoom>();
        services.AddScoped<IUseCase<JoinRoomInput>, JoinRoom>();
        services.AddScoped<IUseCase<LeaveRoomInput>, LeaveRoom>();

        // Presenter。
        services.AddScoped<ListRoomsPresenter>();
        services.AddScoped<IListRoomsOutputPort>(sp => sp.GetRequiredService<ListRoomsPresenter>());
        services.AddScoped<CreateRoomPresenter>();
        services.AddScoped<ICreateRoomOutputPort>(sp => sp.GetRequiredService<CreateRoomPresenter>());
        services.AddScoped<JoinRoomPresenter>();
        services.AddScoped<IJoinRoomOutputPort>(sp => sp.GetRequiredService<JoinRoomPresenter>());
        services.AddScoped<LeaveRoomPresenter>();
        services.AddScoped<ILeaveRoomOutputPort>(sp => sp.GetRequiredService<LeaveRoomPresenter>());

        return services;
    }
}
