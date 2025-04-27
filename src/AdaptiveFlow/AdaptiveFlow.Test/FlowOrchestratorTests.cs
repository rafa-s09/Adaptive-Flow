using AdaptiveFlow.Workbook;
using AdaptiveFlow.Interfaces;
using Moq;
using FluentAssertions;

namespace AdaptiveFlow.Tests;

public class FlowOrchestratorTests : IDisposable
{
    private readonly FlowOrchestrator _orchestrator;

    public FlowOrchestratorTests()
    {
        _orchestrator = new FlowOrchestrator();
    }

    public void Dispose()
    {
        _orchestrator.Dispose();
    }

    [Fact]
    public async Task AddStep_ShouldAddStepToStepsList()
    {
        // Arrange
        var stepMock = new Mock<IFlowStep>();
        var stepName = "Step1";

        // Act
        await _orchestrator.AddStep(stepMock.Object, stepName);

        // Assert
        var steps = _orchestrator.GetSteps().ToList();
        steps.Should().HaveCount(1);
        steps[0].Name.Name.Should().Be(stepName);
        steps[0].Step.Should().Be(stepMock.Object);
        steps[0].Dependencies.Should().BeEmpty();
        steps[0].Condition.Should().BeNull();
        steps[0].AllowParallelExecution.Should().BeFalse();
    }

    [Fact]
    public async Task AddStep_WithDependencies_ShouldAddStepWithDependencies()
    {
        // Arrange
        var stepMock = new Mock<IFlowStep>();
        var stepName = "Step1";
        var dependencies = new[] { "Dep1", "Dep2" };

        // Act
        await _orchestrator.AddStep(stepMock.Object, stepName, dependencies);

        // Assert
        var steps = _orchestrator.GetSteps().ToList();
        steps.Should().HaveCount(1);
        steps[0].Dependencies.Should().HaveCount(2);
        steps[0].Dependencies.Select(d => d.Name).Should().BeEquivalentTo(dependencies);
    }

    [Fact]
    public async Task AddParallelStep_ShouldAddStepWithParallelExecution()
    {
        // Arrange
        var stepMock = new Mock<IFlowStep>();
        var stepName = "Step1";

        // Act
        await _orchestrator.AddParallelStep(stepMock.Object, stepName);

        // Assert
        var steps = _orchestrator.GetSteps().ToList();
        steps.Should().HaveCount(1);
        steps[0].AllowParallelExecution.Should().BeTrue();
    }

    [Fact]
    public async Task AddConditionalStep_ShouldAddStepWithCondition()
    {
        // Arrange
        var stepMock = new Mock<IFlowStep>();
        var stepName = "Step1";
        Func<FlowContext, bool> condition = _ => true;

        // Act
        await _orchestrator.AddConditionalStep(stepMock.Object, stepName, condition);

        // Assert
        var steps = _orchestrator.GetSteps().ToList();
        steps.Should().HaveCount(1);
        steps[0].Condition.Should().NotBeNull();
        steps[0].Condition!(new FlowContext()).Should().BeTrue();
    }

    [Fact]
    public async Task AddStepWithDependencies_ShouldAddStepWithDependencies()
    {
        // Arrange
        var stepMock = new Mock<IFlowStep>();
        var stepName = "Step1";
        var dependencies = new[] { "Dep1", "Dep2" };

        // Act
        await _orchestrator.AddStepWithDependencies(stepMock.Object, stepName, dependencies);

        // Assert
        var steps = _orchestrator.GetSteps().ToList();
        steps.Should().HaveCount(1);
        steps[0].Dependencies.Select(d => d.Name).Should().BeEquivalentTo(dependencies);
    }

    [Fact]
    public void Dispose_ShouldClearSteps()
    {
        // Arrange
        var stepMock = new Mock<IFlowStep>();
        _orchestrator.AddStep(stepMock.Object, "Step1").GetAwaiter().GetResult();

        // Act
        _orchestrator.Dispose();

        // Assert
        _orchestrator.GetSteps().Should().BeEmpty();
    }
}