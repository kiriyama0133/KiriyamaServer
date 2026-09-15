using KiriyamaServer.Application.Boundaries.CloseAccount;
using KiriyamaServer.Application.Exceptions;
using Xunit;

namespace KiriyamaServer.UnitTests.InputValidationTests;
public sealed class CloseAccountInputValidationTests
{
    [Fact]
    public void GivenEmptyAccountId_InputNotCreated_ThrowsInputValidationException()
    {
        var actualEx = Assert.Throws<InputValidationException>(
            () => new CloseAccountInput(
                Guid.Empty));

        Assert.Contains("accountId", actualEx.Message);
    }

    [Fact]
    public void GivenValidData_InputCreated()
    {
        var actual = new CloseAccountInput(Guid.NewGuid());
        Assert.NotNull(actual);
    }
}