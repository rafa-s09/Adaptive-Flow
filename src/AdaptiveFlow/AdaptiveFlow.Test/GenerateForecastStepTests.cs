using AdaptiveFlow.Tests.Steps;

namespace AdaptiveFlow.Tests;

public class GenerateForecastStepTests
{
    [Fact]
    public async Task ExecuteAsync_GeneratesFiveForecasts()
    {
        // Arrange
        var step = new GenerateForecastStep();
        var context = new FlowContext();
        context.Data["StartDate"] = DateTime.Now;

        // Act
        var forecasts = await step.ExecuteAsync(context, CancellationToken.None);

        // Assert
        Assert.NotNull(forecasts);
        Assert.Equal(5, forecasts.Count());
        Assert.True(context.Data.ContainsKey("Forecasts"));
        Assert.Equal(forecasts, context.Data["Forecasts"]);
        Assert.All(forecasts, f => Assert.True(f.TemperatureC >= -20 && f.TemperatureC <= 55));
    }
}
