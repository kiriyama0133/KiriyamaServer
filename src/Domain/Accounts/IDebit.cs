using KiriyamaServer.Domain.ValueObjects;
using Genocs.Common.Domain.Entities;

namespace KiriyamaServer.Domain.Accounts;

public interface IDebit : IEntity<Guid>
{
    PositiveMoney Sum(PositiveMoney amount);
}