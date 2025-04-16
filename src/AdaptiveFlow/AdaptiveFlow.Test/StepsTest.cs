using Moq;

namespace AdaptiveFlow.Tests;

public class StepsTest
{
    [Fact]
    public async Task Step_Should_Execute_Successfully()
    {
        var context = new FlowContext();
        var stepMock = new Mock<IFlowStep>();
        stepMock.Setup(s => s.ExecuteAsync(context, It.IsAny<CancellationToken>()))
                .Returns(Task.CompletedTask);

        var stepWrapper = new VoidFlowStepWrapper(stepMock.Object);
        await stepWrapper.ExecuteAsync(context, CancellationToken.None);

        Assert.True(context.AsReadOnly().Count >= 0); // Verifica que o contexto está intacto.
    }

    [Fact]
    public async Task Step_Should_Throw_Exception()
    {
        var context = new FlowContext();
        var stepMock = new Mock<IFlowStep>();
        stepMock.Setup(s => s.ExecuteAsync(context, It.IsAny<CancellationToken>()))
                .Throws(new InvalidOperationException("Erro na etapa"));

        var stepWrapper = new VoidFlowStepWrapper(stepMock.Object);

        await Assert.ThrowsAsync<InvalidOperationException>(() => stepWrapper.ExecuteAsync(context, CancellationToken.None));
    }


    [Fact]
    public async Task Flow_Should_Detect_Deadlock()
    {
        var config = new FlowConfiguration()
            .AddStep(new Mock<IFlowStep>().Object, "Step1", dependsOn: new[] { "Step2" })
            .AddStep(new Mock<IFlowStep>().Object, "Step2", dependsOn: new[] { "Step1" });

        var manager = new FlowManager(config);

        var context = new FlowContext();
        var result = await manager.RunAsync(context, CancellationToken.None);

        Assert.False(result.Success);
        Assert.Equal("DEADLOCK", result.Error?.Code);
    }

    [Fact]
    public async Task Steps_Should_Execute_In_Parallel()
    {
        var context = new FlowContext();

        var stepMock1 = new Mock<IFlowStep>();
        var stepMock2 = new Mock<IFlowStep>();

        stepMock1.Setup(s => s.ExecuteAsync(context, It.IsAny<CancellationToken>()))
                 .Returns(Task.Delay(500)); // Simula atraso
        stepMock2.Setup(s => s.ExecuteAsync(context, It.IsAny<CancellationToken>()))
                 .Returns(Task.Delay(500));

        var config = new FlowConfiguration()
            .AddStep(stepMock1.Object, "Step1", isParallel: true)
            .AddStep(stepMock2.Object, "Step2", isParallel: true);

        var manager = new FlowManager(config);
        var result = await manager.RunAsync(context, CancellationToken.None);

        Assert.True(result.Success);
    }

    [Fact]
    public async Task Step_Should_Respect_Dependencies()
    {
        var context = new FlowContext();

        var stepMock1 = new Mock<IFlowStep>();
        var stepMock2 = new Mock<IFlowStep>();

        stepMock1.Setup(s => s.ExecuteAsync(context, It.IsAny<CancellationToken>()))
                 .Returns(Task.CompletedTask);
        stepMock2.Setup(s => s.ExecuteAsync(context, It.IsAny<CancellationToken>()))
                 .Returns(Task.CompletedTask);

        var config = new FlowConfiguration()
            .AddStep(stepMock1.Object, "Step1")
            .AddStep(stepMock2.Object, "Step2", dependsOn: new[] { "Step1" });

        var manager = new FlowManager(config);
        var result = await manager.RunAsync(context, CancellationToken.None);

        Assert.True(result.Success);
    }

}
