using AdaptiveFlow.Tests.Steps;
using Microsoft.Extensions.DependencyInjection;
using System.Reflection.Metadata;

namespace AdaptiveFlow.Tests;

public class FlowConfigurationTests
{
    private readonly IServiceProvider _serviceProvider;

    public FlowConfigurationTests()
    {
        var services = new ServiceCollection();
        services.AddTransient<GenerateForecastStep>();
        services.AddTransient<LogForecastStep>();
        _serviceProvider = services.BuildServiceProvider();
    }

    [Fact]
    public void AddStep_ConfiguresStepsCorrectly()
    {
        // Arrange
        var config = new FlowConfiguration()
            .AddStep(_serviceProvider.GetRequiredService<GenerateForecastStep>(), "Generate")
            .AddStep(_serviceProvider.GetRequiredService<LogForecastStep>(), "Log", dependsOn: new[] { "Generate" });

        // Act
        var steps = config.Steps;

        // Assert
        Assert.Equal(2, steps.Count);
        Assert.Equal("Generate", steps[0].StepName);
        Assert.Equal("Log", steps[1].StepName);
        Assert.Contains("Generate", steps[1].DependsOn);
    }

    [Fact]
    public void FromJson_ConfiguresStepsCorrectly()
    {
        // Arrange
        string jsonConfig = """[{"StepType": "GenerateForecastStep", "StepName": "Generate", "IsParallel": false},{"StepType": "LogForecastStep", "StepName": "Log", "IsParallel": false, "DependsOn": ["Generate"]}]""";

        var stepRegistry = new Dictionary<string, Type>
            {
                { "GenerateForecastStep", typeof(GenerateForecastStep) },
                { "LogForecastStep", typeof(LogForecastStep) }
            };

        // Act
        var config = FlowConfiguration.FromJson(jsonConfig, _serviceProvider, stepRegistry);
        var steps = config.Steps;

        // Assert
        Assert.Equal(2, steps.Count);
        Assert.Equal("Generate", steps[0].StepName);        
        Assert.Equal("Log", steps[1].StepName);
        Assert.Contains("Generate", steps[1].DependsOn);
    }
}
