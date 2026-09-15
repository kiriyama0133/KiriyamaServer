using KiriyamaServer.Domain.Accounts;
using KiriyamaServer.Domain.ValueObjects;

namespace KiriyamaServer.Infrastructure.PersistenceLayer.PostgreSQL;

public class Debit : Domain.Accounts.Debit
{
    public Guid AccountId { get; protected set; }

    protected Debit()
    {
    }

    public Debit(IAccount account, PositiveMoney amountToWithdraw, DateTime transactionDate)
    {
        AccountId = account.Id;
        Amount = amountToWithdraw;
        TransactionDate = transactionDate;
    }
}