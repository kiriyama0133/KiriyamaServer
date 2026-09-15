using KiriyamaServer.Application.Interfaces;
using KiriyamaServer.WebApi.UseCases.V1.CloseAccount;
using KiriyamaServer.WebApi.UseCases.V1.Deposit;
using KiriyamaServer.WebApi.UseCases.V1.GetAccountDetails;
using KiriyamaServer.WebApi.UseCases.V1.GetCustomerDetails;
using KiriyamaServer.WebApi.UseCases.V1.Refund;
using KiriyamaServer.WebApi.UseCases.V1.Register;
using KiriyamaServer.WebApi.UseCases.V1.Transfer;
using KiriyamaServer.WebApi.UseCases.V1.Withdraw;

namespace KiriyamaServer.WebApi.Extensions;

public static class UserInterfaceV1Extensions
{
    public static IServiceCollection AddPresentersV1(this IServiceCollection services)
    {

        services.AddScoped<CloseAccountPresenter, CloseAccountPresenter>();
        services.AddScoped<IOutputPort<Application.Boundaries.CloseAccount.CloseAccountOutput>>(x => x.GetRequiredService<CloseAccountPresenter>());
        services.AddScoped<DepositPresenter, DepositPresenter>();
        services.AddScoped<IOutputPort<Application.Boundaries.Deposits.DepositOutput>>(x => x.GetRequiredService<DepositPresenter>());
        services.AddScoped<GetAccountDetailsPresenter, GetAccountDetailsPresenter>();
        services.AddScoped<Application.Boundaries.GetAccountDetails.IOutputPort>(x => x.GetRequiredService<GetAccountDetailsPresenter>());
        services.AddScoped<IOutputPort<Application.Boundaries.GetAccountDetails.GetAccountDetailsOutput>>(x => x.GetRequiredService<GetAccountDetailsPresenter>());
        services.AddScoped<GetCustomerDetailsPresenter, GetCustomerDetailsPresenter>();
        services.AddScoped<Application.Boundaries.GetCustomerDetails.IOutputPort>(x => x.GetRequiredService<GetCustomerDetailsPresenter>());
        services.AddScoped<IOutputPort<Application.Boundaries.GetCustomerDetails.GetCustomerDetailsOutput>>(x => x.GetRequiredService<GetCustomerDetailsPresenter>());
        services.AddScoped<RegisterPresenter, RegisterPresenter>();
        services.AddScoped<Application.Boundaries.Registers.IOutputPort>(x => x.GetRequiredService<RegisterPresenter>());
        services.AddScoped<WithdrawPresenter, WithdrawPresenter>();
        services.AddScoped<IOutputPort<Application.Boundaries.Withdraws.WithdrawOutput>>(x => x.GetRequiredService<WithdrawPresenter>());
        services.AddScoped<RefundPresenter, RefundPresenter>();
        services.AddScoped<Application.Boundaries.Refunds.IOutputPort>(x => x.GetRequiredService<RefundPresenter>());
        services.AddScoped<TransferPresenter, TransferPresenter>();
        services.AddScoped<IOutputPort<Application.Boundaries.Transfers.TransferOutput>>(x => x.GetRequiredService<TransferPresenter>());
        return services;
    }
}
