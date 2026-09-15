using KiriyamaServer.Domain.ValueObjects;

namespace KiriyamaServer.Domain.Accounts;

public class Credit : ICredit
{
    public Guid Id { get; protected set; } = Guid.NewGuid();
    public static string Description => "Credit";
    public PositiveMoney Amount { get; protected set; } = new PositiveMoney(0);
    public DateTime TransactionDate { get; protected set; } = DateTime.UtcNow;

    public PositiveMoney Sum(PositiveMoney amount)
        => Amount.Add(amount);

    public bool IsTransient()
    {
        throw new NotImplementedException();
    }
}