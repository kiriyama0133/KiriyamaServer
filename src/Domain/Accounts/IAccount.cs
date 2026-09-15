using KiriyamaServer.Domain.ValueObjects;
using Genocs.Common.Domain.Entities;

namespace KiriyamaServer.Domain.Accounts;

public interface IAccount : IAggregateRoot<Guid>
{
    ICredit Deposit(IEntityFactory entityFactory, PositiveMoney amountToDeposit);
    IDebit? Withdraw(IEntityFactory entityFactory, PositiveMoney amountToWithdraw);
    bool IsClosingAllowed();
    Money GetCurrentBalance();
}