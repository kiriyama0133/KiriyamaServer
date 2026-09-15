using KiriyamaServer.Application.Boundaries.CloseAccount;
using System.Collections.ObjectModel;

namespace KiriyamaServer.Infrastructure.PersistenceLayer.Presenters;

public sealed class CloseAccountPresenter : IOutputPort
{
    public Collection<string> Errors { get; }
    public Collection<CloseAccountOutput> ClosedAccounts { get; }

    public CloseAccountPresenter()
    {
        Errors = [];
        ClosedAccounts = [];
    }

    public void Error(string message)
        => Errors.Add(message);

    public void Default(CloseAccountOutput output)
        => ClosedAccounts.Add(output);
}