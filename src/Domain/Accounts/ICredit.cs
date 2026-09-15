using KiriyamaServer.Domain.ValueObjects;
using Genocs.Common.Domain.Entities;

namespace KiriyamaServer.Domain.Accounts;

public interface ICredit : IEntity<Guid>
{
    PositiveMoney Sum(PositiveMoney amount);
}