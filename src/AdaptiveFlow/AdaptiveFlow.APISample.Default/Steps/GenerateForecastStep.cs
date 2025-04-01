using AdaptiveFlow.APISample.Default.Models;

namespace AdaptiveFlow.APISample.Default.Steps
{
    public class GenerateForecastStep : IFlowStep<IEnumerable<WeatherForecastModel>>
    {
        private static readonly string[] Summaries =
        [
            "Freezing", "Bracing", "Chilly", "Cool", "Mild", "Warm", "Balmy", "Hot", "Sweltering", "Scorching"
        ];

        public async Task<IEnumerable<WeatherForecastModel>> ExecuteAsync(FlowContext context, CancellationToken cancellationToken)
        {
            var forecasts = Enumerable.Range(1, 5).Select(index => new WeatherForecastModel
            {
                Date = DateTime.Now.AddDays(index),
                TemperatureC = Random.Shared.Next(-20, 55),
                Summary = Summaries[Random.Shared.Next(Summaries.Length)]
            }).ToArray();

            context.Data["Forecasts"] = forecasts;
            return await Task.FromResult(forecasts);
        }
    }
}
