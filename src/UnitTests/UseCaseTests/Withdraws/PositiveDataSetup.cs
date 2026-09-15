using Xunit;

namespace KiriyamaServer.UnitTests.UseCaseTests.Withdraws;

internal sealed class PositiveDataSetup : TheoryData<decimal, decimal>
{
    public PositiveDataSetup()
    {
        Add(100, 600);
    }
}