using KiriyamaServer.Application.Exceptions;
using KiriyamaServer.Application.Interfaces;
using KiriyamaServer.Domain.ValueObjects;

namespace KiriyamaServer.Application.Boundaries.Deposits;

public sealed class DepositInput : IInputType
{
    public Guid AccountId { get; }
    public PositiveMoney Amount { get; }

    public DepositInput(in Guid accountId, PositiveMoney? amount)
    {
        if (accountId == Guid.Empty)
        {
            throw new InputValidationException($"{nameof(accountId)} cannot be empty.");
        }

        AccountId = accountId;
        Amount = amount ?? throw new InputValidationException($"{nameof(amount)} cannot be null.");
    }
}