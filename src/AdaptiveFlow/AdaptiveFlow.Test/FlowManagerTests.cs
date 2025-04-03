using AdaptiveFlow.Tests.Steps;
using Microsoft.Extensions.Logging;
using Moq;

namespace AdaptiveFlow.Tests;

public class FlowManagerTests
{
    private readonly ILogger<FlowManager> _mockLogger;

    public FlowManagerTests()
    {
        var loggerMock = new Mock<ILogger<FlowManager>>();
        _mockLogger = loggerMock.Object;
    }

    [Fact]
    public async Task RunAsync_SuccessfulExecution_ReturnsSuccess()
    {
        // Arrange
        var config = new FlowConfiguration()
            .AddStep(new SuccessStep(), "SuccessStep");
        var manager = new FlowManager(config, _mockLogger);
        var context = new FlowContext();

        // Act
        var result = await manager.RunAsync(context);

        // Assert
        Assert.True(result.Success);
        Assert.True(string.IsNullOrEmpty(result.ErrorMessage));
        Assert.True(context.Data.ContainsKey("Success"));
        Assert.Empty(result.Result.StepResults);
    }

    [Fact]
    public async Task RunAsync_FailingStep_ReturnsFailure()
    {
        // Arrange
        var config = new FlowConfiguration()
            .AddStep(new FailingStep(), "FailingStep");
        var manager = new FlowManager(config, _mockLogger);
        var context = new FlowContext();

        // Act
        var result = await manager.RunAsync(context);

        // Assert
        Assert.False(result.Success);
        Assert.Contains("Step failed intentionally", result.ErrorMessage);
        Assert.Null(result.Result);
    }

    [Fact]
    public async Task RunAsync_CancellationRequested_ThrowsAndReturnsCanceled()
    {
        // Arrange
        var config = new FlowConfiguration()
            .AddStep(new DelayedStep(1000), "DelayedStep");
        var manager = new FlowManager(config, _mockLogger);
        var context = new FlowContext();
        var cts = new CancellationTokenSource();
        cts.CancelAfter(200);

        // Act
        var result = await manager.RunAsync(context, cts.Token);

        // Assert
        Assert.False(result.Success);
        Assert.Equal("Flow was canceled", result.ErrorMessage);
        Assert.Null(result.Result);
    }

    [Fact]
    public async Task RunAsync_ParallelSteps_ExecutesConcurrently()
    {
        // Arrange
        var config = new FlowConfiguration()
            .AddStep(new DelayedStep(300), "Step1", isParallel: true)
            .AddStep(new DelayedStep(300), "Step2", isParallel: true);
        var manager = new FlowManager(config, _mockLogger, maxParallelism: 2);
        var context = new FlowContext();

        // Act
        var startTime = DateTime.UtcNow;
        var result = await manager.RunAsync(context);
        var duration = DateTime.UtcNow - startTime;

        // Assert
        Assert.True(result.Success);
        Assert.True(duration.TotalMilliseconds < 500, "Steps should run in parallel, taking ~300ms");
        Assert.True(context.Data.ContainsKey("Delayed"));
    }

    [Fact]
    public async Task RunAsync_DependencyDeadlock_ReturnsFailure()
    {
        // Arrange
        var config = new FlowConfiguration()
            .AddStep(new SuccessStep(), "Step1", dependsOn: ["Step2"])
            .AddStep(new SuccessStep(), "Step2", dependsOn: ["Step1"]);
        var manager = new FlowManager(config, _mockLogger);
        var context = new FlowContext();

        // Act
        var result = await manager.RunAsync(context);

        // Assert
        Assert.False(result.Success);
        Assert.Contains("Deadlock detected", result.ErrorMessage);
        Assert.Null(result.Result);
    }

    [Fact]
    public async Task RunAsync_TypedStep_ReturnsResult()
    {
        // Arrange
        var config = new FlowConfiguration()
            .AddStep(new ComputeStep(), "ComputeStep");
        var manager = new FlowManager(config, _mockLogger);
        var context = new FlowContext();

        // Act
        var result = await manager.RunAsync(context);

        // Assert
        Assert.True(result.Success);
        Assert.Single(result.Result.StepResults);
        Assert.Equal(42, result.Result.StepResults[0]);
    }

    [Fact]
    public async Task EnqueueAsync_ChannelFull_WaitsAndProcesses()
    {
        // Arrange
        var config = new FlowConfiguration()
            .AddStep(new DelayedStep(100), "DelayedStep");
        var manager = new FlowManager(config, _mockLogger, channelCapacity: 1);
        var processingTask = manager.StartProcessingAsync(); // Inicia o processamento em segundo plano
        var context1 = new FlowContext();
        var context2 = new FlowContext();

        // Act
        await manager.EnqueueAsync(context1); // Enfileira o primeiro contexto
        var enqueueTask = manager.EnqueueAsync(context2); // Enfileira o segundo, deve esperar
        await Task.Delay(150); // Dá tempo para o primeiro contexto ser processado
        await enqueueTask; // Aguarda o segundo ser enfileirado

        // Aguarda o processamento terminar com timeout
        await Task.WhenAny(processingTask, Task.Delay(100)); // Timeout de 1 segundo

        // Assert
        Assert.True(context1.Data.ContainsKey("Delayed"), "O primeiro contexto não foi processado.");
        Assert.True(context2.Data.ContainsKey("Delayed"), "O segundo contexto não foi processado.");
    }
}