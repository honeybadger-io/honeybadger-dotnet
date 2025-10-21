using Honeybadger;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Options;

var builder = new ConfigurationBuilder();
var baseDir = Directory.GetCurrentDirectory();
var configuration = builder
    .SetBasePath(baseDir)
    .AddJsonFile("appsettings.json", false, false)
    .AddUserSecrets<Program>()
    .Build();

var options = new HoneybadgerOptions();
configuration
    .GetSection("Honeybadger")
    .Bind(options);

var client = new HoneybadgerClient(Options.Create(options));
await client.NotifyAsync("Hello from .Net! Improved error handling with Honeybadger.");
Console.WriteLine("Done. Check your Honeybadger dashboard for the error.");

internal partial class Program
{
}