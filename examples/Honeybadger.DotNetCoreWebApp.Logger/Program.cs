using Honeybadger;
using Honeybadger.DotNetCore;
using Honeybadger.Extensions.Logging;
using Microsoft.AspNetCore.Mvc;

var builder = WebApplication.CreateBuilder(args);
builder.AddHoneybadger();
builder.Logging.AddHoneybadger();

var app = builder.Build();

app.MapGet("/", (IEnumerable<EndpointDataSource> endpointSources) =>
{
    var routes = string.Join("\n", endpointSources.SelectMany(source => source.Endpoints));
    return $"Hello World! Visit the following routes to test:\n{routes}";
});

app.MapGet("/notify", ([FromServices] ILogger<Program> logger, IHoneybadgerClient hb) =>
{
    var shouldReport = hb.Options.ShouldReport();
    if (!shouldReport)
    {
        return "Log won't be reported to Honeybadger, " +
               "because its api key is not set or the environment is in the list of DevelopmentEnvironments.";
    }
    
    logger.LogInformation("Hello from Honeybadger.Logger!");
    
    return "Log should be reported to Honeybadger. Check your dashboard!";
});

app.MapGet("/throw", ([FromServices] ILogger<Program> logger, IHoneybadgerClient hb) =>
{
    if (!hb.Options.ShouldReport())
    {
        throw new Exception("This exception should not be reported to Honeybadger because of either ApiKey or AppEnvironment.");
    }
    
    if (!hb.Options.ReportUnhandledExceptions)
    {
        throw new Exception("This exception should not be reported because ReportUnhandledExceptions is false.");    
    }
    
    throw new Exception("This exception should be reported to Honeybadger.");
});


app.Run();

public partial class Program {}