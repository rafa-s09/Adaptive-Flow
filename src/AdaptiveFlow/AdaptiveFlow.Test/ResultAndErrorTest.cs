using Microsoft.Extensions.Logging;
using Moq;

namespace AdaptiveFlow.Tests;

public class ResultAndErrorTest
{
    [Fact]
    public void FlowResult_Should_Be_Built_Correctly()
    {
        var error = FlowError.Exception("Step1", new Exception("Erro genérico"));
        var successResult = FlowResult.Ok(new Dictionary<string, object?> { { "Key", "Value" } });
        var failResult = FlowResult.Fail(error);

        Assert.True(successResult.Success);
        Assert.NotNull(successResult.Results);
        Assert.False(failResult.Success);
        Assert.NotNull(failResult.Error);
    }

    [Fact]
    public void FlowError_Should_Be_Built_Correctly()
    {
        var deadlockError = FlowError.Deadlock("Step1, Step2");
        var exceptionError = FlowError.Exception("Step1", new Exception("Erro inesperado"));

        Assert.Equal("DEADLOCK", deadlockError.Code);
        Assert.Equal("EXCEPTION", exceptionError.Code);
    }

}
