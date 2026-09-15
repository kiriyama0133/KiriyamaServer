using KiriyamaServer.Application.Exceptions;
using KiriyamaServer.Application.Interfaces;
using KiriyamaServer.Domain.ValueObjects;

namespace KiriyamaServer.Application.Boundaries.Withdraws;

public sealed class WithdrawInput : IInputType
{
    public Guid AccountId { get; }
    public PositiveMoney Amount { get; }

    public WithdrawInput(Guid accountId, PositiveMoney? amount)
    {
        if (accountId == Guid.Empty)
        {
            throw new InputValidationException($"{nameof(accountId)} cannot be empty.");
        }

        if (amount == null)
        {
            throw new InputValidationException($"{nameof(amount)} cannot be null.");
        }

        AccountId = accountId;
        Amount = amount;
    }
}