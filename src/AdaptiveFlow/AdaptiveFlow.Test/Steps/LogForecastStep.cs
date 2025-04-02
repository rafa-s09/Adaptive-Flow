using AdaptiveFlow.Tests.Models;

namespace AdaptiveFlow.Tests.Steps;

public class LogForecastStep : IFlowStep
{
    public async Task ExecuteAsync(FlowContext context, CancellationToken cancellationToken)
    {
        if (context.Data.TryGetValue("Forecasts", out var forecastsObj) && forecastsObj is IEnumerable<WeatherForecastModel> forecasts)
        {
            foreach (var forecast in forecasts)
            {
                Console.WriteLine($"Previsão para {forecast.Date}: {forecast.TemperatureC}°C, {forecast.Summary}");
            }
        }
        await Task.CompletedTask;
    }
}

