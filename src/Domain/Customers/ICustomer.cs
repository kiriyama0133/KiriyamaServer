using KiriyamaServer.Domain.Accounts;
using Genocs.Common.Domain.Entities;

namespace KiriyamaServer.Domain.Customers;

public interface ICustomer : IAggregateRoot<Guid>
{
    AccountCollection Accounts { get; }
    void Register(IAccount account);
}