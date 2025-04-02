using AdaptiveFlow.Tests.Models;
using AdaptiveFlow.Tests.Steps;
using Microsoft.Extensions.DependencyInjection;

namespace AdaptiveFlow.Tests;

public class FlowManagerTests
{
    private readonly IServiceProvider _serviceProvider;

    public FlowManagerTests()
    {
        var services = new ServiceCollection();
        services.AddTransient<GenerateForecastStep>();
        services.AddTransient<LogForecastStep>();
        _serviceProvider = services.BuildServiceProvider();
    }

    [Fact]
    public async Task RunAsync_ExecutesStepsInOrder_ReturnsSuccess()
    {
        // Arrange
        var config = new FlowConfiguration()
            .AddStep(_serviceProvider.GetRequiredService<GenerateForecastStep>(), "Generate")
            .AddStep(_serviceProvider.GetRequiredService<LogForecastStep>(), "Log", dependsOn: new[] { "Generate" });
        var flowManager = new FlowManager(config);
        var context = new FlowContext();
        context.Data["StartDate"] = DateTime.Now;

        // Act
        var result = await flowManager.RunAsync(context, CancellationToken.None);

        // Assert
        Assert.True(result.Success);
        Assert.True(string.IsNullOrEmpty(result.ErrorMessage));
        Assert.NotNull(result.Result);
        var innerResults = (FlowInnerResults)result.Result;
        var forecasts = innerResults.StepResults.OfType<IEnumerable<WeatherForecastModel>>().FirstOrDefault();
        Assert.NotNull(forecasts);
        Assert.Equal(5, forecasts.Count());
        Assert.True(context.Data.ContainsKey("Forecasts")); // Verifica se o LogForecastStep teve acesso
    }

    [Fact]
    public async Task RunAsync_WithJsonConfig_ReturnsSuccess()
    {
        // Arrange
        string jsonConfig = """[{"StepType": "GenerateForecastStep", "StepName": "Generate", "IsParallel": false},{"StepType": "LogForecastStep", "StepName": "Log", "IsParallel": false, "DependsOn": ["Generate"]}]""";

        var stepRegistry = new Dictionary<string, Type>
            {
                { "GenerateForecastStep", typeof(GenerateForecastStep) },
                { "LogForecastStep", typeof(LogForecastStep) }
            };
        var config = FlowConfiguration.FromJson(jsonConfig, _serviceProvider, stepRegistry);
        var flowManager = new FlowManager(config);
        var context = new FlowContext();
        context.Data["StartDate"] = DateTime.Now;

        // Act
        var result = await flowManager.RunAsync(context, CancellationToken.None);

        // Assert
        Assert.True(result.Success);
        Assert.True(string.IsNullOrEmpty(result.ErrorMessage));
        Assert.NotNull(result.Result);
        var innerResults = (FlowInnerResults)result.Result;
        var forecasts = innerResults.StepResults.OfType<IEnumerable<WeatherForecastModel>>().FirstOrDefault();
        Assert.NotNull(forecasts);
        Assert.Equal(5, forecasts.Count());
    }

    [Fact]
    public async Task RunAsync_WithDeadlock_ReturnsFailure()
    {
        // Arrange
        var config = new FlowConfiguration()
            .AddStep(_serviceProvider.GetRequiredService<GenerateForecastStep>(), "Generate", dependsOn: new[] { "Log" })
            .AddStep(_serviceProvider.GetRequiredService<LogForecastStep>(), "Log", dependsOn: new[] { "Generate" });
        var flowManager = new FlowManager(config);
        var context = new FlowContext();

        // Act
        var result = await flowManager.RunAsync(context, CancellationToken.None);

        // Assert
        Assert.False(result.Success);
        Assert.Contains("Deadlock detected", result.ErrorMessage);
        Assert.Null(result.Result);
    }
}