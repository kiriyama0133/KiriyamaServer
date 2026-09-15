using Xunit;

namespace KiriyamaServer.UnitTests.UseCaseTests.Deposits;

internal sealed class NegativeDataSetup : TheoryData<decimal>
{
    public NegativeDataSetup()
    {
        Add(-100);
    }
}