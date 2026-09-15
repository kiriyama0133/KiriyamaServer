using KiriyamaServer.Application.Repositories;
using KiriyamaServer.Domain.Customers;
using Microsoft.EntityFrameworkCore;

namespace KiriyamaServer.Infrastructure.PersistenceLayer.PostgreSQL.Repositories;

public sealed class CustomerRepository(GenocsContext context) : ICustomerRepository
{
    private readonly GenocsContext _context = context ?? throw new ArgumentNullException(nameof(context));

    public async Task AddAsync(ICustomer customer, CancellationToken cancellationToken = default)
    {
        await _context.Customers.AddAsync((Customer)customer, cancellationToken);
        await _context.SaveChangesAsync(cancellationToken);
    }

    public async Task<ICustomer?> GetAsync(Guid id, CancellationToken cancellationToken = default)
    {
        var customer = await _context.Customers
            .Where(c => c.Id == id)
            .SingleOrDefaultAsync(cancellationToken);

        if (customer == null)
        {
            return null;
        }

        List<Guid> accounts = [.. _context.Accounts
            .Where(e => e.CustomerId == id)
            .Select(e => e.Id)];

        customer.LoadAccounts(accounts);

        return customer;
    }

    public async Task UpdateAsync(ICustomer customer, CancellationToken cancellationToken = default)
    {
        _context.Customers.Update((Customer)customer);
        await _context.SaveChangesAsync(cancellationToken);
    }
}