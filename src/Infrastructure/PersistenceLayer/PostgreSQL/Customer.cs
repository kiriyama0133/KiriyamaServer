using KiriyamaServer.Domain.Customers;
using KiriyamaServer.Domain.ValueObjects;

namespace KiriyamaServer.Infrastructure.PersistenceLayer.PostgreSQL;

public class Customer : Domain.Customers.Customer
{
    protected Customer()
    {
    }

    public Customer(SSN ssn, Name name)
    {
        Id = Guid.NewGuid();
        SSN = ssn;
        Name = name;
    }

    public void LoadAccounts(IEnumerable<Guid> accounts)
    {
        Accounts = new AccountCollection();
        Accounts.Add(accounts);
    }
}