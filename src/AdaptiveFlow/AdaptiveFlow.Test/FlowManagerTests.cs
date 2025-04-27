using AdaptiveFlow.Workbook;
using AdaptiveFlow.Interfaces;
using Moq;
using FluentAssertions;
using AdaptiveFlow.Exceptions;

namespace AdaptiveFlow.Tests;

public class FlowManagerTests
{
    private readonly Mock<IFlowStep> _stepMock;
    private readonly FlowOrchestrator _orchestrator;
    private readonly FlowContext _context;

    public FlowManagerTests()
    {
        _stepMock = new Mock<IFlowStep>();
        _orchestrator = new FlowOrchestrator();
        _context = new FlowContext();
    }

    [Fact]
    public async Task ExecuteAsync_SingleStep_SuccessfulExecution_ReturnsSuccessResult()
    {
        // Arrange
        var stepName = "Step1";
        _stepMock.Setup(s => s.ExecuteAsync(It.IsAny<FlowContext>(), It.IsAny<CancellationToken>()))
            .ReturnsAsync("Result");
        await _orchestrator.AddStep(_stepMock.Object, stepName);
        var manager = new FlowManager(_orchestrator);

        // Act
        var results = await manager.ExecuteAsync(_context);

        // Assert
        results.Should().HaveCount(1);
        results[0].StepName.Name.Should().Be(stepName);
        results[0].Success.Should().BeTrue();
        results[0].Content.Should().Be("Result");
        results[0].Message.Should().BeNull();
    }

    [Fact]
    public async Task ExecuteAsync_StepThrowsException_ReturnsFailureResult()
    {
        // Arrange
        var stepName = "Step1";
        var exception = new Exception("Step failed");
        _stepMock.Setup(s => s.ExecuteAsync(It.IsAny<FlowContext>(), It.IsAny<CancellationToken>()))
            .ThrowsAsync(exception);
        await _orchestrator.AddStep(_stepMock.Object, stepName);
        var manager = new FlowManager(_orchestrator);

        // Act
        var results = await manager.ExecuteAsync(_context);

        // Assert
        results.Should().HaveCount(1);
        results[0].StepName.Name.Should().Be(stepName);
        results[0].Success.Should().BeFalse();
        results[0].Content.Should().BeNull();
        results[0].Message.Should().BeOfType<FlowManagerException>();
        results[0].Message!.InnerException.Should().Be(exception);
    }

    [Fact]
    public async Task ExecuteAsync_WithDependencies_ExecutesInCorrectOrder()
    {
        // Arrange
        var step1Name = "Step1";
        var step2Name = "Step2";
        var step1Mock = new Mock<IFlowStep>();
        var step2Mock = new Mock<IFlowStep>();
        step1Mock.Setup(s => s.ExecuteAsync(It.IsAny<FlowContext>(), It.IsAny<CancellationToken>()))
            .ReturnsAsync("Result1");
        step2Mock.Setup(s => s.ExecuteAsync(It.IsAny<FlowContext>(), It.IsAny<CancellationToken>()))
            .ReturnsAsync("Result2");
        await _orchestrator.AddStep(step1Mock.Object, step1Name);
        await _orchestrator.AddStep(step2Mock.Object, step2Name, new[] { step1Name });
        var manager = new FlowManager(_orchestrator);

        // Act
        var results = await manager.ExecuteAsync(_context);

        // Assert
        results.Should().HaveCount(2);
        results[0].StepName.Name.Should().Be(step1Name);
        results[1].StepName.Name.Should().Be(step2Name);
    }

    [Fact]
    public void Constructor_DuplicateStepNames_ThrowsFlowOrchestrationException()
    {
        // Arrange
        _orchestrator.AddStep(_stepMock.Object, "Step1").GetAwaiter().GetResult();
        _orchestrator.AddStep(_stepMock.Object, "Step1").GetAwaiter().GetResult();

        // Act & Assert
        Assert.Throws<FlowOrchestrationException>(() => new FlowManager(_orchestrator));
    }

    [Fact]
    public void Constructor_MissingDependency_ThrowsFlowOrchestrationException()
    {
        // Arrange
        _orchestrator.AddStep(_stepMock.Object, "Step1", new[] { "MissingDep" }).GetAwaiter().GetResult();

        // Act & Assert
        Assert.Throws<FlowOrchestrationException>(() => new FlowManager(_orchestrator));
    }

    [Fact]
    public async Task ExecuteAsync_DeadlockDetected_ThrowsFlowOrchestrationException()
    {
        // Arrange
        await _orchestrator.AddStep(_stepMock.Object, "Step1", new[] { "Step2" });
        await _orchestrator.AddStep(_stepMock.Object, "Step2", new[] { "Step1" });
        var manager = new FlowManager(_orchestrator);

        // Act & Assert
        await Assert.ThrowsAsync<FlowOrchestrationException>(() => manager.ExecuteAsync(_context));
    }
}





