using KiriyamaServer.Domain.ValueObjects;

namespace KiriyamaServer.Domain.Accounts;

public class Debit : IDebit
{
    public Guid Id { get; protected set; } = Guid.NewGuid();
    public PositiveMoney Amount { get; protected set; } = new PositiveMoney(0);
    public static string Description => "Debit";
    public DateTime TransactionDate { get; protected set; }

    public PositiveMoney Sum(PositiveMoney amount)
    {
        return Amount.Add(amount);
    }

    public bool IsTransient()
    {
        throw new NotImplementedException();
    }
}