using KiriyamaServer.Application.Interfaces;
using KiriyamaServer.Domain.Accounts;
using KiriyamaServer.Domain.ValueObjects;

namespace KiriyamaServer.Application.Boundaries.Withdraws;

public sealed class WithdrawOutput : IOutputType
{
    public Transaction Transaction { get; }
    public decimal UpdatedBalance { get; }

    public WithdrawOutput(IDebit debit, Money updatedBalance)
    {
        Debit debitEntity = (Debit)debit;

        Transaction = new Transaction(
            Debit.Description,
            debitEntity.Amount
            .ToMoney()
            .ToDecimal(),
            debitEntity.TransactionDate);

        UpdatedBalance = updatedBalance.ToDecimal();
    }
}