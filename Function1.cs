using Azure.Core;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Azure.Functions.Worker;
using Microsoft.Extensions.Logging;
using System.Net;
using System.Threading.Tasks;

namespace WeatherFunctionApp
{
  public class Function1
  {
    private readonly ILogger<Function1> _logger;
    private readonly IMyService _my_service;

    public Function1(ILogger<Function1> logger, IMyService myService)
    {    
      _logger = logger;
      _my_service = myService;
    }

    [Function("Function1")]    
    public async Task<IActionResult> Run([HttpTrigger(AuthorizationLevel.Anonymous, "get", "post")] HttpRequest req)
    {
      var summaries = new[]
      {
        "Freezing", "Bracing", "Chilly", "Cool", "Mild", "Warm", "Balmy", "Hot", "Sweltering", "Scorching"
      };

      try
      {
        _logger.LogInformation("C# HTTP trigger function processed a request.");
        _logger.LogInformation("Asking NASA for some asteriod information");
        var response = await _my_service.MyServiceMethodAsync(new MyServiceRequest { Request = "Hello" });

        _logger.LogInformation($"Nasa informed {response.Response} items");                

        var forecast = Enumerable.Range(1, 5).Select(index =>
            new WeatherForecast
            (
                DateOnly.FromDateTime(DateTime.Now.AddDays(index)),
                Random.Shared.Next(-20, 55),
                summaries[Random.Shared.Next(summaries.Length)]
            ))
            .ToArray();

        return new OkObjectResult(forecast);
      }
      catch (Exception ex)
      {
        _logger.LogError($"Something bad happened {ex.Message}");
        return new StatusCodeResult(StatusCodes.Status500InternalServerError);
      }      
    }
  }

  record WeatherForecast(DateOnly Date, int TemperatureC, string? Summary)
  {
    public int TemperatureF => 32 + (int)(TemperatureC / 0.5556);
  }
}
