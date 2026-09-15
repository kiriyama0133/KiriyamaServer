using KiriyamaServer.Application.Repositories;
using KiriyamaServer.Application.Services;
using KiriyamaServer.Domain;
using KiriyamaServer.UnitTests.TestFixtures;
using Microsoft.EntityFrameworkCore;
using KiriyamaServer.Infrastructure.PersistenceLayer.PostgreSQL;
using KiriyamaServer.Infrastructure.PersistenceLayer.PostgreSQL.Repositories;

namespace KiriyamaServer.UnitTests;

public sealed class TestFixture
{
    public TestFixture()
    {
        PostgreSQLFixture();
    }

    public IEntityFactory EntityFactory { get; private set; }
    public GenocsContext Context { get; private set; }
    public IAccountRepository AccountRepository { get; private set; }
    public ICustomerRepository CustomerRepository { get; private set; }
    public IUnitOfWork UnitOfWork { get; private set; }
    public IServiceBusClient ServiceBus { get; private set; }
    private void PostgreSQLFixture()
    {
        var options = new DbContextOptionsBuilder<GenocsContext>()
            .UseInMemoryDatabase(databaseName: "test_database")
            .Options;

        Context = new GenocsContext(options);
        AccountRepository = new AccountRepository(Context);
        CustomerRepository = new CustomerRepository(Context);
        UnitOfWork = new UnitOfWork(Context);
        EntityFactory = new EntityFactory();
        ServiceBus = new FakeServiceBus();
    }

    public static TestFixture Instance { get; } = new TestFixture();
}
