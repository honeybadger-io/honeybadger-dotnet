# Honeybadger for .Net Apps

This is the .Net Honeybadger Notifier.

## Supported .Net versions:

| Family | Version |
|--------|-----|
| .Net | 6.0 |
|.Net Standard| n/a |
|.Net Core| n/a |
|.Net Framework| n/a |

## Getting Started

Clone/Fork. Since the project is currently at MVP stage and not yet published on nuget.org, the best way to use it would be to
clone or fork it and build as a local project in your solution.

### For .Net Core Web App

1. Reference the `Honeybadger.DotNetCoreWebApp` project.
2. Register the _Honeybadger Middleware_:
```c#
var builder = WebApplication.CreateBuilder(args);
builder.Services.AddHoneybadger();
```

You can get access to the _Honeybadger Client_ using _DI_:
```c#
app.MapGet("/", ([FromServices] IHoneybadgerClient client) =>
{
    client.AddBreadcrumb("reached index route", "route", new Dictionary<string, object?>());
    
    return "Hello World!";
});
```
3. Any unhandled exceptions should be reported to Honeybadger automatically:
```c#
app.MapGet("/debug", () =>
{
    throw new Exception("hello from .Net Core Web App!");
});
```

See example project in `examples/Honeybadger.DotNetCoreWebApp`.

### As a custom logging provider 

1. Reference the `Honeybadger.Extensions.Logging` project.
2. Register the custom logging provider:
```c#
var builder = WebApplication.CreateBuilder(args);
builder.Logging.AddHoneybadger();
```
3. Errors from the `logger` will be reported to Honeybadger:
```c#
app.MapGet("/notify", ([FromServices] ILogger logger) =>
{
    logger.LogError("hello from Honeybadger.Logger!");
    
    return "Log reported to Honeybadger. Check your dashboard!";
});
```

See example project in `examples/Honeybadger.DotNetCoreWebApp.Logger`.

### Using the client manually

1. Reference the `Honeybadger` project.
2. Initialize the _Honeybadger Client_:
```c#
var client = HoneybadgerSdk.Init(new HoneybadgerOptions
{
    AppEnvironment = "development"
});
```
3. Call `notify` to report to Honeybadger:
```c#
client.Notify("hello from .Net !");
```

See example project in `examples/Honeybadger.Console`.

## TODO
- [ ] Publish _.Net Core_ Nuget to nuget.org
- [ ] Publish _Honeybadger Logging Provider_ Nuget to nuget.org
- [ ] Target .Net 5.0
- [ ] Target .Net Core 3.1
- [ ] Target .Net Standard 2.1
- [ ] Target .Net Standard 2.0 (?)
- [ ] Support more features of the [Honeybadger Client Library Spec](https://www.notion.so/honeybadger/Client-Library-Spec-aa891332f7874196aa0695b6d38dca66). Tracked [here](https://pc-hiteq-software.monday.com/boards/1981156519/). 

### References
- Create a [class library](https://docs.microsoft.com/en-us/nuget/quickstart/create-and-publish-a-package-using-the-dotnet-cli) project
- StackTrack class
- [Honeybadger Client Library Spec](https://www.notion.so/honeybadger/Client-Library-Spec-aa891332f7874196aa0695b6d38dca66)
- [Honeybadger Crystal](https://github.com/honeybadger-io/honeybadger-crystal) - Recently Released
