using KiriyamaServer.Application.Exceptions;
using KiriyamaServer.Application.Interfaces;

namespace KiriyamaServer.Application.Boundaries.CloseAccount;

public sealed class CloseAccountInput : IInputType
{
    public Guid AccountId { get; }

    public CloseAccountInput(in Guid accountId)
    {
        if (accountId == Guid.Empty)
        {
            throw new InputValidationException($"{nameof(accountId)} cannot be empty.");
        }

        AccountId = accountId;
    }
}