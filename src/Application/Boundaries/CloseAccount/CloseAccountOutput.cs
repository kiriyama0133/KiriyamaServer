using KiriyamaServer.Application.Interfaces;
using KiriyamaServer.Domain.Accounts;

namespace KiriyamaServer.Application.Boundaries.CloseAccount;

public sealed class CloseAccountOutput(IAccount account) : IOutputType
{
    public Guid AccountId { get; } = account.Id;
}