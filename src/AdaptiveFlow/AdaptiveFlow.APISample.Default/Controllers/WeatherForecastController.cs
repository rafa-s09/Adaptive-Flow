using AdaptiveFlow.APISample.Models;
using AdaptiveFlow.APISample.Steps;
using Microsoft.AspNetCore.Mvc;

namespace AdaptiveFlow.APISample.Controllers
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

        [HttpGet("DefaultStepWorkGroup")]
        public async Task<IActionResult> Get(CancellationToken cancellationToken)
        {
            var config = new FlowConfiguration()
                .AddStep(_serviceProvider.GetRequiredService<GenerateForecastStep>(), "Generate")
                .AddStep(_serviceProvider.GetRequiredService<LogForecastStep>(), "Log", dependsOn: ["Generate"]);
            var flowManager = new FlowManager(config);

            var context = new FlowContext();
            var result = await flowManager.RunAsync(context, cancellationToken);

            if (result.Success && result.Result != null)
            {
                FlowInnerResults innerResults = result.Result;
                var forecastCollection = innerResults.StepResults.OfType<IEnumerable<WeatherForecastModel>>().FirstOrDefault();
                return Ok(forecastCollection ?? []);
            }

            return BadRequest(result.ErrorMessage ?? "Unknown error occurred");
        }

        [HttpGet("FromJsonWorkGroup")]
        public async Task<IActionResult> GetFromJson(CancellationToken cancellationToken)
        {
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

            var result = await flowManager.RunAsync(context, cancellationToken);
            if (result.Success && result.Result != null)
            {
                FlowInnerResults innerResults = result.Result;
                var forecastCollection = innerResults.StepResults.OfType<IEnumerable<WeatherForecastModel>>().FirstOrDefault();
                return Ok(forecastCollection ?? []);
            }
            return BadRequest(result.ErrorMessage ?? "Unknown error occurred");
        }
    }
}
