using KiriyamaServer.Domain.Customers;
using KiriyamaServer.Domain.ValueObjects;
using KiriyamaServer.Infrastructure.PersistenceLayer.PostgreSQL.Repositories;
using KiriyamaServer.Infrastructure.PersistenceLayer.PostgreSQL;
using Microsoft.EntityFrameworkCore;
using Xunit;

namespace KiriyamaServer.IntegrationTests.PostgreSQL;

public sealed class CustomerRepositoryTests
{
    private readonly EntityFactory _entityFactory = new();

    [Fact]
    public async Task Add_ChangesDatabaseAsync()
    {
        var customer = _entityFactory.NewCustomer(
            new SSN("8608179992"),
            new Name("Nocco Vincenzo"));

        await using var context = BuildContext();
        context.Database.EnsureCreated();

        var repository = new CustomerRepository(context);

        await repository.AddAsync(customer);

        var customers = await context.Customers.ToListAsync();
        Assert.Equal(2, customers.Count);
        Assert.Contains(customers, c => c.Id == customer.Id);
    }

    [Fact]
    public async Task Get_ReturnsCustomerAsync()
    {
        await using var context = BuildContext();
        context.Database.EnsureCreated();

        var customer1 = _entityFactory.NewCustomer(
            new SSN("8608179994"),
            new Name("Nocco Roberto"));

        var repository = new CustomerRepository(context);
        await repository.AddAsync(customer1);

        ICustomer? customer2 = await repository.GetAsync(customer1.Id);

        Assert.NotNull(customer2);
    }

    private GenocsContext BuildContext()
    {
        var options = new DbContextOptionsBuilder<GenocsContext>()
            .UseInMemoryDatabase(databaseName: "test_database")
            .Options;

        return new GenocsContext(options);
    }
}