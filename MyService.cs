using System.Net.Http.Json;
using System.Text.Json;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using WeatherFunctionApp;
using Nasa.Neo;
using Microsoft.Extensions.Primitives;

public class MyServiceRequest
{
  public string? Request { get; set; }
}

public class MyServiceResponse
{
  public string? Response { get; set; }
  public string? NeoResponse { get; set; }
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

    string neoResponseString = string.Empty;

    try
    {
      using (var nasa = await _httpClient.GetAsync(""))
      {
        _logger.LogInformation("NASA response: {Response}", nasa.StatusCode);

        nasa.EnsureSuccessStatusCode();

        var options = new JsonSerializerOptions { PropertyNameCaseInsensitive = true };


        //var json = await nasa.Content.ReadAsStringAsync();
        //var data = JsonSerializer.Deserialize<NeoResponse>(json);

        var stream = await nasa.Content.ReadAsStreamAsync();
        var data = await JsonSerializer.DeserializeAsync<NeoResponse>(stream);

        if (data == null)
        {
          _logger.LogError("NASA response is null");
          throw new Exception("NASA response is null");
        }
        if (data.NearEarthObjects == null || data.NearEarthObjects.Count == 0)
        {
          _logger.LogError("NASA response has no NearEarthObjects");
          throw new Exception("NASA response has no NearEarthObjects");
        } 

        neoResponseString = $"NASA response: {data.NearEarthObjects.Count} objects";  

        _logger.LogInformation("NASA response: {Response}", neoResponseString);

      }

      return await Task.FromResult(new MyServiceResponse() { 
        Response = "Hello from MyService", NeoResponse= neoResponseString
      });
    }
    catch (HttpIOException ex)
    {
      _logger.LogError(ex, "Error in MyServiceMethodAsync: {Message}", ex.Message);
      throw;
    }
    catch (HttpRequestException ex) when (ex.StatusCode == System.Net.HttpStatusCode.NotFound)
    {
      _logger.LogError(ex, "Error NASA Service cannot be reached in MyServiceMethodAsync: {Message}", ex.Message);
      throw;
    }
    catch (Exception ex)
    {
      _logger.LogError(ex, "Unexpected Error in MyServiceMethodAsync: {Message}", ex.Message);
      throw;
    }
  }

}
