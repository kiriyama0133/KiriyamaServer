using KiriyamaServer.Application.Interfaces;
using KiriyamaServer.Domain.Accounts;
using KiriyamaServer.Domain.ValueObjects;

namespace KiriyamaServer.Application.Boundaries.Deposits;

public sealed class DepositOutput : IOutputType
{
    public Transaction Transaction { get; }
    public decimal UpdatedBalance { get; }

    public DepositOutput(ICredit credit, Money updatedBalance)
    {
        Credit creditEntity = (Credit)credit;

        Transaction = new Transaction(
                                    Credit.Description,
                                    creditEntity
                                    .Amount
                                    .ToMoney()
                                    .ToDecimal(),
                                    creditEntity.TransactionDate);

        UpdatedBalance = updatedBalance.ToDecimal();
    }
}