using Microsoft.Azure.Functions.Worker;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;

var host = new HostBuilder()
    .ConfigureFunctionsWebApplication()
    .ConfigureLogging( config => {
      config.ClearProviders();
      config.AddSimpleConsole(c =>
      {
        c.SingleLine = true;
        c.ColorBehavior = Microsoft.Extensions.Logging.Console.LoggerColorBehavior.Default;
      });
      config.AddJsonConsole(c =>
      {
        c.IncludeScopes = true;
        c.TimestampFormat = "yyyy-MM-dd HH:mm:ss.fff zzz";
        c.UseUtcTimestamp = true;        
      });
    })
    .ConfigureServices(services =>
    {      
      services.AddApplicationInsightsTelemetryWorkerService();
      services.ConfigureFunctionsApplicationInsights();
      services.AddHttpClient<IMyService, MyService>( client => {
        client.BaseAddress = new Uri("https://api.nasa.gov/neo/rest/v1/neo/browse?api_key=DEMO_KEY");
        client.DefaultRequestHeaders.UserAgent.ParseAdd("dotnet-docs");
      });
    })
    .Build();

host.Run();