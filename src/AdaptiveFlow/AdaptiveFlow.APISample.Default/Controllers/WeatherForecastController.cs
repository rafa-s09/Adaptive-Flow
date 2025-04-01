using AdaptiveFlow.APISample.Default.Models;
using AdaptiveFlow.APISample.Default.Steps;
using Microsoft.AspNetCore.Mvc;
using System.Text.Json;

namespace AdaptiveFlow.APISample.Default.Controllers
{
    [ApiController]
    [Route("[controller]")]
    public class WeatherForecastController : ControllerBase
    {
        private readonly IServiceProvider _serviceProvider;

        public WeatherForecastController(IServiceProvider serviceProvider)
        {
            _serviceProvider = serviceProvider;
        }

        [HttpGet]
        public async Task<IActionResult> Get(CancellationToken cancellationToken)
        {
            var config = new FlowConfiguration()
                .AddStep(_serviceProvider.GetRequiredService<GenerateForecastStep>(), "Generate")
                .AddStep(_serviceProvider.GetRequiredService<LogForecastStep>(), "Log", dependsOn: ["Generate"]);
            var flowManager = new FlowManager(config);

            var context = new FlowContext();
            var result = await flowManager.RunAsync(context, cancellationToken);

            string jsonString = JsonSerializer.Serialize(result.Result);

            if (result.Success && result.Result != null)
            {
                dynamic dynamicResult = result.Result;
                var stepResults = (IList<object>)dynamicResult.StepResults.FirstOrDefault(); // Lista com [IEnumerable<WeatherForecast>]
                var forecastCollection = stepResults.FirstOrDefault() as IEnumerable<WeatherForecastModel>; // Pega o primeiro item como a coleção
                return Ok(forecastCollection ?? Array.Empty<WeatherForecastModel>());
            }

            if (result.Success && result.Result != null)
            {
                // Acessa os forecasts diretamente do ContextData
                if (context.Data.TryGetValue("Forecasts", out var forecastsObj) && forecastsObj is IEnumerable<WeatherForecastModel> forecasts)
                {
                    return Ok(forecasts);
                }
                return Ok(Array.Empty<WeatherForecastModel>()); // Fallback se não houver forecasts
            }

            return BadRequest(result.ErrorMessage);
        }
    }
}
