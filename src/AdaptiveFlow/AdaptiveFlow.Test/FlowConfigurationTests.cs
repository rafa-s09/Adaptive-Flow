using AdaptiveFlow.Tests.Steps;
using Microsoft.Extensions.DependencyInjection;

namespace AdaptiveFlow.Tests;

public class FlowConfigurationTests
{
    [Fact]
    public void AddStep_VoidStep_AddsCorrectly()
    {
        // Arrange
        var config = new FlowConfiguration();
        var step = new SuccessStep();

        // Act
        config.AddStep(step, "TestStep");

        // Assert
        Assert.Single(config.Steps);
        var (Step, StepName, Condition, IsParallel, DependsOn) = config.Steps[0];
        Assert.Equal("TestStep", StepName);
        Assert.IsType<VoidFlowStepWrapper>(Step);
        Assert.True(Condition(new FlowContext())); // Condição padrão é sempre true
        Assert.False(IsParallel);
        Assert.Empty(DependsOn);
    }

    [Fact]
    public void AddStep_TypedStep_AddsCorrectly()
    {
        // Arrange
        var config = new FlowConfiguration();
        var step = new ComputeStep();

        // Act
        config.AddStep(step, "ComputeStep");

        // Assert
        Assert.Single(config.Steps);
        var (Step, StepName, Condition, IsParallel, DependsOn) = config.Steps[0];
        Assert.Equal("ComputeStep", StepName);
        Assert.IsType<TypedFlowStepWrapper<int>>(Step);
        Assert.True(Condition(new FlowContext()));
        Assert.False(IsParallel);
        Assert.Empty(DependsOn);
    }

    [Fact]
    public void AddStep_WithConditionAndParallel_AddsWithCustomSettings()
    {
        // Arrange
        var config = new FlowConfiguration();
        var step = new SuccessStep();
        Func<FlowContext, bool> condition = ctx => ctx.Data.ContainsKey("Key");

        // Act
        config.AddStep(step, "ConditionalStep", condition, isParallel: true, dependsOn: ["OtherStep"]);

        // Assert
        Assert.Single(config.Steps);
        var (Step, StepName, Condition, IsParallel, DependsOn) = config.Steps[0];
        Assert.Equal("ConditionalStep", StepName);
        Assert.False(Condition(new FlowContext())); // Falso sem a chave

        var context = new FlowContext();
        context.Data["Key"] = "Value";
        Assert.True(Condition(context)); // Verdadeiro com a chave
        Assert.True(IsParallel);
        Assert.Contains("OtherStep", DependsOn);
    }

    [Fact]
    public void AddSteps_MultipleSteps_AddsAllCorrectly()
    {
        // Arrange
        var config = new FlowConfiguration();
        var steps = new (IFlowStepWrapper Step, string StepName)[]
        {
                (new VoidFlowStepWrapper(new SuccessStep()), "Step1"),
                (new TypedFlowStepWrapper<int>(new ComputeStep()), "Step2")
        };
        Func<FlowContext, bool> condition = ctx => true;

        // Act
        config.AddSteps(steps, condition, isParallel: true, dependsOn: ["InitialStep"]);

        // Assert
        Assert.Equal(2, config.Steps.Count);
        var (Step1, StepName1, Condition1, IsParallel1, DependsOn1) = config.Steps[0];
        var (Step2, StepName2, Condition, IsParallel2, DependsOn2) = config.Steps[1];
        Assert.Equal("Step1", StepName1);
        Assert.IsType<VoidFlowStepWrapper>(Step1);
        Assert.True(IsParallel1);
        Assert.Contains("InitialStep", DependsOn1);
        Assert.Equal("Step2", StepName2);
        Assert.IsType<TypedFlowStepWrapper<int>>(Step2);
        Assert.True(IsParallel2);
        Assert.Contains("InitialStep", DependsOn2);
    }

    [Fact]
    public void AddSteps_EmptyCollection_DoesNotThrow()
    {
        // Arrange
        var config = new FlowConfiguration();
        var steps = Array.Empty<(IFlowStepWrapper, string)>();

        // Act
        config.AddSteps(steps);

        // Assert
        Assert.Empty(config.Steps);
    }

    [Fact]
    public void FromJson_ValidConfig_CreatesFlowConfiguration()
    {
        // Arrange
        var json = @"[
                {""StepType"": ""SuccessStep"", ""StepName"": ""Step1"", ""IsParallel"": false},
                {""StepType"": ""ComputeStep"", ""StepName"": ""Step2"", ""DependsOn"": [""Step1""]}
            ]";
        var stepRegistry = new Dictionary<string, Type>
            {
                { "SuccessStep", typeof(SuccessStep) },
                { "ComputeStep", typeof(ComputeStep) }
            };
        var services = new ServiceCollection()
            .AddSingleton<SuccessStep>()
            .AddSingleton<ComputeStep>()
            .BuildServiceProvider();

        // Act
        var config = FlowConfiguration.FromJson(json, services, stepRegistry);

        // Assert
        Assert.Equal(2, config.Steps.Count);
        Assert.Equal("Step1", config.Steps[0].StepName);
        Assert.IsType<VoidFlowStepWrapper>(config.Steps[0].Step);
        Assert.Equal("Step2", config.Steps[1].StepName);
        Assert.IsType<TypedFlowStepWrapper<int>>(config.Steps[1].Step);
        Assert.Contains("Step1", config.Steps[1].DependsOn);
    }

    [Fact]
    public void FromJson_UnregisteredStep_ThrowsArgumentException()
    {
        // Arrange
        var json = @"[{""StepType"": ""UnknownStep"", ""StepName"": ""Step1""}]";
        var stepRegistry = new Dictionary<string, Type>();
        var services = new ServiceCollection().BuildServiceProvider();

        // Act & Assert
        var exception = Assert.Throws<ArgumentException>(() =>
            FlowConfiguration.FromJson(json, services, stepRegistry));
        Assert.Contains("not registered", exception.Message);
    }

    [Fact]
    public void FromJson_InvalidJson_ThrowsArgumentException()
    {
        // Arrange
        var json = "invalid json";
        var stepRegistry = new Dictionary<string, Type>();
        var services = new ServiceCollection().BuildServiceProvider();

        // Act & Assert
        var exception = Assert.Throws<ArgumentException>(() =>
            FlowConfiguration.FromJson(json, services, stepRegistry));
        Assert.Contains("Invalid JSON", exception.Message);
    }
}