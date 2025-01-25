using Microsoft.AspNetCore.Mvc;

var builder = WebApplication.CreateBuilder(args);
builder.Logging.AddHoneybadger();

var app = builder.Build();

app.MapGet("/", () => "Hello World!");
app.MapGet("/notify", ([FromServices] ILogger<Program> logger) =>
{
    logger.LogError("Hello from Honeybadger.Logger! Improved error handling with Honeybadger.");
    
    return "Log reported to Honeybadger. Check your dashboard!";
});

app.Run();

public partial class Program {}