using Microsoft.Azure.Functions.Worker;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Logging.Console;

var host = new HostBuilder()
    .ConfigureFunctionsWebApplication()
    .ConfigureLogging(config =>
    {      
      config.AddSimpleConsole(c =>
      {
          c.ColorBehavior = LoggerColorBehavior.Default;
          c.IncludeScopes = true;
          c.SingleLine = true;          
      });
    })
    .ConfigureServices(services =>
    {
        services.AddApplicationInsightsTelemetryWorkerService();
        services.ConfigureFunctionsApplicationInsights();
        services.AddHttpClient<IMyService, MyService>(client =>
        {
            client.BaseAddress = new Uri("https://api.nasa.gov/neo/rest/v1/neo/browse?api_key=DEMO_KEY");
            client.DefaultRequestHeaders.UserAgent.ParseAdd("dotnet-docs");
        });
    })
    .Build();
  
host.Run();