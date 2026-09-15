using KiriyamaServer.Domain.Customers;

namespace KiriyamaServer.Application.Repositories;

public interface ICustomerRepository
{
    Task<ICustomer?> GetAsync(Guid id, CancellationToken cancellationToken = default);
    Task AddAsync(ICustomer customer, CancellationToken cancellationToken = default);
    Task UpdateAsync(ICustomer customer, CancellationToken cancellationToken = default);
}