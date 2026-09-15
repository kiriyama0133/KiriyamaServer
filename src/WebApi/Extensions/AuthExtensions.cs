using KiriyamaServer.Application.Boundaries.Auth;
using KiriyamaServer.Application.Interfaces;
using KiriyamaServer.Application.Repositories;
using KiriyamaServer.Application.Services;
using KiriyamaServer.Application.UseCases;
using KiriyamaServer.Infrastructure.PersistenceLayer.PostgreSQL.Repositories;
using KiriyamaServer.Infrastructure.Security;
using KiriyamaServer.WebApi.UseCases.Auth;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;

namespace KiriyamaServer.WebApi.Extensions;

/// <summary>鉴权（注册/登录/令牌）的依赖注入装配。</summary>
public static class AuthExtensions
{
    public static IServiceCollection AddAuth(this IServiceCollection services, IConfiguration configuration)
    {
        // 仓储。
        services.AddScoped<IUserRepository, UserRepository>();

        // 密码哈希与令牌服务。
        services.AddSingleton<IPasswordHasher, Pbkdf2PasswordHasher>();
        services.Configure<JwtOptions>(configuration.GetSection(JwtOptions.Position));
        services.AddSingleton<ITokenService, JwtTokenService>();

        // 用例。
        services.AddScoped<IUseCase<RegisterInput>, RegisterUser>();
        services.AddScoped<IUseCase<LoginInput>, Login>();
        services.AddScoped<IUseCase<TokenInput>, Token>();

        // Presenter。
        services.AddScoped<RegisterPresenter>();
        services.AddScoped<IRegisterOutputPort>(sp => sp.GetRequiredService<RegisterPresenter>());
        services.AddScoped<LoginPresenter>();
        services.AddScoped<ILoginOutputPort>(sp => sp.GetRequiredService<LoginPresenter>());
        services.AddScoped<TokenPresenter>();
        services.AddScoped<ITokenOutputPort>(sp => sp.GetRequiredService<TokenPresenter>());

        return services;
    }
}
