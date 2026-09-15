using KiriyamaServer.Application.Interfaces;
using KiriyamaServer.Application.Services;
using KiriyamaServer.Application.UseCases;
using KiriyamaServer.Infrastructure.WebApiClient.ExternalServices;

namespace KiriyamaServer.WebApi.Extensions;

public static class ApplicationExtensions
{
    public static IServiceCollection AddUseCases(this IServiceCollection services)
    {

        services.AddScoped<IApiClient, AuthApiClient>();

        services.AddScoped<IUseCase<Application.Boundaries.CloseAccount.CloseAccountInput>, CloseAccount>();
        services.AddScoped<IUseCase<Application.Boundaries.Deposits.DepositInput>, Deposit>();
        services.AddScoped<IUseCase<Application.Boundaries.GetAccountDetails.GetAccountDetailsInput>, GetAccountDetails>();
        services.AddScoped<IUseCase<Application.Boundaries.Refunds.RefundInput>, Refund>();
        services.AddScoped<IUseCase<Application.Boundaries.GetCustomerDetails.GetCustomerDetailsInput>, GetCustomerDetails>();
        services.AddScoped<IUseCase<Application.Boundaries.Registers.RegisterInput>, Register>();
        services.AddScoped<IUseCase<Application.Boundaries.Withdraws.WithdrawInput>, Withdraw>();
        services.AddScoped<IUseCase<Application.Boundaries.Transfers.TransferInput>, Transfer>();
        return services;
    }
}
