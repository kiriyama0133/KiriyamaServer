using KiriyamaServer.Application.Exceptions;
using KiriyamaServer.Application.Interfaces;

namespace KiriyamaServer.Application.Boundaries.GetAccountDetails;

public sealed class GetAccountDetailsInput : IInputType
{
    public Guid AccountId { get; }

    public GetAccountDetailsInput(in Guid accountId)
    {
        if (accountId == Guid.Empty)
        {
            throw new InputValidationException($"{nameof(accountId)} cannot be empty.");
        }

        AccountId = accountId;
    }
}