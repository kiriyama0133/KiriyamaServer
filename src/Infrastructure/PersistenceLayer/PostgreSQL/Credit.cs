using KiriyamaServer.Domain.Accounts;
using KiriyamaServer.Domain.ValueObjects;

namespace KiriyamaServer.Infrastructure.PersistenceLayer.PostgreSQL;

public class Credit : Domain.Accounts.Credit
{
    public Guid AccountId { get; protected set; }

    protected Credit()
    {
    }

    public Credit(IAccount account, PositiveMoney amountToDeposit, DateTime transactionDate)
    {
        AccountId = account.Id;
        Amount = amountToDeposit;
        TransactionDate = transactionDate;
    }
}