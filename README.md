# Honeybadger for .Net Apps

This is the .Net Honeybadger Notifier.

## Supported .Net versions:

All modern .Net Core applications are supported, up to .Net 9.0.

## Getting Started

### Configuration

The Honeybadger Notifier can be configured using the `HoneybadgerOptions` class
(or `HoneybadgerLoggingOptions` when using as a logger).
Honeybadger can be configured by passing the options when registering the service,
or through your `appsettings.json` file.

Honeybadger, by default, will not report errors in development environments.
You can override the development environments by setting the `DevelopmentEnvironments` property in the options.
Alternatively, you can set the `ReportData` property to `true` to report errors in all environments.

See below for examples on how to configure Honeybadger for different types of applications.

### For .Net Core Web App

1. Install Honeybadger.DotNetCore from Nuget
   ```
   dotnet add package Honeybadger.DotNetCore
   ```
2. Register the _Honeybadger Middleware_:
   ```c#
   var builder = WebApplication.CreateBuilder(args);
   builder.AddHoneybadger(new HoneybadgerOptions("api_key"));
   ```
   
   Or you can configure Honeybadger through your `appsettings.json` file, by adding a `Honeybadger` section:
   ```json
   {
     "Honeybadger": {
       "ApiKey": "_api_key",
       "AppEnvironment": "Development",
       "ReportData": true 
     }
   }
   ```
   And simply call `AddHoneybadger` without any parameters:
   ```c#
    var builder = WebApplication.CreateBuilder(args);
    builder.AddHoneybadger();
   ```

#### Usage

You can get access to the _Honeybadger Client_ using _DI_:
```c#
app.MapGet("/", ([FromServices] IHoneybadgerClient client) =>
{
    client.AddBreadcrumb("reached index route", "route", new Dictionary<string, object?>());
    
    return "Hello World!";
});
```

Any unhandled exceptions should be reported to Honeybadger automatically:
```c#
app.MapGet("/debug", () =>
{
    throw new Exception("hello from .Net Core Web App!");
});
```

See example project in `examples/Honeybadger.DotNetCoreWebApp`.

### As a custom logging provider 

1. Install Honeybadger.Extensions.Logging from Nuget
   ```
   dotnet add package Honeybadger.Extensions.Logging
   ```
2. Register the custom logging provider:
   ```c#
   var builder = WebApplication.CreateBuilder(args);
   builder.Logging.AddHoneybadger(new HoneybadgerLoggingOptions 
   {
       ApiKey = "api_key",
       Environment = "Development",
       ReportData = true,
       MinimumLogLevel = LogLevel.Error,
       MinimumBreadcrumbLevel = LogLevel.Information
   });
   ```

   Or you can configure Honeybadger through your `appsettings.json` file, by adding a `Honeybadger` section inside the `Logging` section:
   ```json
   {
     "Logging": {
       "Honeybadger": {
          "ApiKey": "_api_key",
          "AppEnvironment": "Development",
          "ReportData": true,
          "MinimumLogLevel": "Error",
          "MinimumBreadcrumbLevel": "Information"
       }
     }
   }
   ```
   And simply call `AddHoneybadger` without any parameters:
   ```c#
    var builder = WebApplication.CreateBuilder(args);
    builder.Logging.AddHoneybadger();
   ```

#### Usage

Errors from the `logger` will be reported to Honeybadger:
   ```c#
   app.MapGet("/notify", ([FromServices] ILogger logger) =>
   {
       logger.LogError("hello from Honeybadger.Logger!");
       
       return "Log reported to Honeybadger. Check your dashboard!";
   });
   ```

See example project in `examples/Honeybadger.DotNetCoreWebApp.Logger`.

### Using the SDK manually

1. Install the [Honeybadger Nuget](https://www.nuget.org/packages/Honeybadger).
   ```
   dotnet add package Honeybadger
   ```
2. Initialize the _Honeybadger Client_:
   ```c#
   using Microsoft.Extensions.Options;
   
   var options = new HoneybadgerOptions("apiKey");
   var client = new HoneybadgerClient(Options.Create(options));
   ```
3. Call `notify` to report to Honeybadger:
   ```c#
   client.Notify("hello from .Net !");
   ```

See example project in `examples/Honeybadger.Console`.

## Changelog

Changelog is automatically generated using [Conventional Commits](https://www.conventionalcommits.org/) with [versionize](https://github.com/versionize/versionize).
Conventional Commits are enforced with a pre-commit git hook (using [husky](https://alirezanet.github.io/Husky.Net/guide/)).

## Contributing

1. Fork the repo.
2. Create a topic branch git checkout -b my_branch
3. Commit your changes git commit -am "chore: boom"
4. Write a test that verifies your changes
5. Push to your branch git push origin my_branch
6. Send a [pull request](https://github.com/honeybadger-io/honeybadger-dotnet/pulls)
7. Make sure that CI checks are passing

## Releasing

All packages are published on nuget.org with a [Github Actions Worfklow](./.github/workflows/release.yml).
The workflow does the following:
- `dotnet versionize` - bump versions and generate changelog
- `dotnet pack`
- `dotnet package push`

_Note: only users with write permissions can trigger this workflow (i.e. Collaborators)._

## TODO

- [ ] Publish README with basic info to setup core nuget
- [ ] Publish Honeybadger.DotNetCore with README
- [ ] Publish Honeybadger.Extensions.Logging with README
- [ ] Implement Error Grouping (custom fingerprint)
- [ ] Implement Error Tags
- [ ] Allow excluding errors (either with a BeforeNotify method or exception classes config)
- [ ] Implement Filter Keys (exclude sensitive keys)
- [ ] Implement Checkins
- [ ] Implement Collect User Feedback
- [ ] Create guide for Deployment Tracking
- [ ] Create integration guide in honeybadger-docs project

## License

This Honeybadger repository and published packages are MIT licensed. See the [MIT-LICENSE](./MIT_LICENSE) file in this repository for details.
