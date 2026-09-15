using System.Collections.ObjectModel;
using KiriyamaServer.Application.Boundaries.Deposits;
using KiriyamaServer.Application.Interfaces;

namespace KiriyamaServer.Infrastructure.PersistenceLayer.Presenters;

public sealed class DepositPresenter : IOutputPort<DepositOutput>
{
    public Collection<string> Errors { get; }
    public Collection<DepositOutput> Deposits { get; }

    public DepositPresenter()
    {
        Errors = [];
        Deposits = [];
    }

    public void Error(string message)
        => Errors.Add(message);

    public void Default(DepositOutput output)
        => Deposits.Add(output);
}