using Honeybadger;
using Honeybadger.DotNetCore;
using Microsoft.AspNetCore.Mvc;

var builder = WebApplication.CreateBuilder(args);
builder.AddHoneybadger();

var app = builder.Build();
app.MapGet("/", ([FromServices] IHoneybadgerClient client) =>
{
    client.AddBreadcrumb("reached index route", "route", new Dictionary<string, object?>());
    return "Hello World!";
});
app.MapGet("/debug", () =>
{
    throw new Exception("hello from .Net Core Web App!");
});

app.Run();