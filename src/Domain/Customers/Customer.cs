using KiriyamaServer.Domain.Accounts;
using KiriyamaServer.Domain.ValueObjects;
using Genocs.Common.CQRS.Events;

namespace KiriyamaServer.Domain.Customers;

public class Customer : ICustomer
{
    public Guid Id { get; protected set; }
    public Name? Name { get; protected set; }
    public SSN? SSN { get; protected set; }
    public AccountCollection Accounts { get; protected set; }

    public IReadOnlyCollection<IEvent> DomainEvents => [];

    public Customer()
    {
        Accounts = new AccountCollection();
    }

    public void Register(IAccount account)
    {
        Accounts ??= new AccountCollection();

        Accounts.Add(account.Id);
    }

    public bool IsTransient()
    {
        throw new NotImplementedException();
    }

    public void AddDomainEvent(IEvent @event)
    {
        throw new NotImplementedException();
    }

    public void ClearDomainEvents()
    {
        throw new NotImplementedException();
    }
}