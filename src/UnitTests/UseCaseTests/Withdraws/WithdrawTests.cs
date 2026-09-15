using KiriyamaServer.Application.Boundaries.Withdraws;
using KiriyamaServer.Application.UseCases;
using KiriyamaServer.Domain.ValueObjects;
using KiriyamaServer.Infrastructure.PersistenceLayer.Presenters;
using Xunit;

namespace KiriyamaServer.UnitTests.UseCaseTests.Withdraws;

public sealed class WithdrawlTests(TestFixture fixture) : IClassFixture<TestFixture>
{
    private readonly TestFixture _fixture = fixture;

    [Theory]
    [ClassData(typeof(PositiveDataSetup))]
    public async Task Withdraw_Valid_AmountAsync(decimal amount, decimal expectedBalance)
    {
        // Arrange - Create and persist customer with account
        var customer = _fixture.EntityFactory.NewCustomer(
            new SSN("198608179933"),
            new Name("Withdraw Test Customer"));

        var account = _fixture.EntityFactory.NewAccount(customer);

        // Add initial deposit to account (expected balance + amount to withdraw)
        decimal initialBalance = expectedBalance + amount;
        var initialCredit = account.Deposit(_fixture.EntityFactory, new PositiveMoney(initialBalance));

        customer.Register(account);

        // Persist customer and account to the in-memory database
        await _fixture.CustomerRepository.AddAsync(customer);
        await _fixture.AccountRepository.AddAsync(account, initialCredit);
        await _fixture.UnitOfWork.SaveAsync();

        var presenter = new WithdrawPresenter();
        var sut = new Withdraw(
            _fixture.EntityFactory,
            presenter,
            _fixture.AccountRepository,
            _fixture.UnitOfWork,
            _fixture.ServiceBus);

        // Act
        await sut.ExecuteAsync(new WithdrawInput(
            account.Id,
            new PositiveMoney(amount)));

        // Assert
        var actual = presenter.Withdrawals.Last();
        Assert.Equal(expectedBalance, actual.UpdatedBalance);
    }
}