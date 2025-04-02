using System.Net.Http.Json;
using System.Text.Json;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using WeatherFunctionApp;
using Nasa.Neo;

public class MyServiceRequest
{
  public string? Request { get; set; }
}

public class MyServiceResponse
{
  public string? Response { get; set; }
}

public interface IMyService
{
  Task<MyServiceResponse> MyServiceMethodAsync(MyServiceRequest request);
}


public class MyService : IMyService, IDisposable
{
  private readonly HttpClient _httpClient;
  private readonly ILogger<MyService> _logger;

  public MyService(HttpClient httpClient, ILogger<MyService> logger)
  {
    _logger = logger;
    _httpClient = httpClient;

    _logger.LogInformation($"MyService object created. {this.GetHashCode()} {_httpClient.GetHashCode()}");
  }

  public void Dispose()
  {
    _logger.LogInformation("MyService Disposed");
    _httpClient?.Dispose();
  }

  public async Task<MyServiceResponse> MyServiceMethodAsync(MyServiceRequest request)
  {
    _logger.LogInformation("Called MyServiceMethodAsync with request: {Request}", request.Request);

    using (var nasa = await _httpClient.GetAsync(""))
    {
      _logger.LogInformation("NASA response: {Response}", nasa.StatusCode);

      nasa.EnsureSuccessStatusCode();

      var options = new JsonSerializerOptions { PropertyNameCaseInsensitive = true };


      //var json = await nasa.Content.ReadAsStringAsync();
      //var data = JsonSerializer.Deserialize<NeoResponse>(json);

      var stream = await nasa.Content.ReadAsStreamAsync();
      var data = await JsonSerializer.DeserializeAsync<NeoResponse>(stream);

      //_logger.LogInformation("NASA response: {Response} Objects", data);

      _logger.LogInformation("NASA response: {Response} Objects", data.NearEarthObjects.Count);

    }

    return await Task.FromResult(new MyServiceResponse() { Response = "Hello from MyService" });
  }

}
