# Honeybadger for .Net Apps

This is the .Net Honeybadger Notifier.

## Supported .Net versions:

All modern .Net Core applications are supported, up to .Net 9.0.

## Getting Started

### Configuration

The Honeybadger Notifier can be configured using the `HoneybadgerOptions` class.
Honeybadger can be configured by passing the options when registering the service,
or through your `appsettings.json` file.

Honeybadger will attempt to automatically figure out the `ProjectRoot` directory, 
which should be the root of your project or solution. A valid `ProjectRoot` directory will allow Honeybadger to
classify stack frames as either _application_ code or _all_ other code (e.g. framework code) 
and hence provide better error reports.

See below for examples on how to configure Honeybadger for different types of applications.

### For .Net Core Web App

#### 1. Install Honeybadger.DotNetCore from Nuget

```sh
dotnet add package Honeybadger.DotNetCore
```

#### 2. Register the _Honeybadger Middleware_:

```c#
var builder = WebApplication.CreateBuilder(args);
builder.AddHoneybadger(configure =>
{
    configure.ApiKey = "{{PROJECT_API_KEY}}";
});
```

Or you can configure Honeybadger through your `appsettings.json` file, by adding a `Honeybadger` section:

```json
{
   "Honeybadger": {
      "ApiKey": "{{PROJECT_API_KEY}}",
      "AppEnvironment": "Development",
      "ReportData": true
   }
}
```

> [!NOTE]
> You should probably set your API key through environment variables or use the Secrets Manager, instead of hardcoding it in the `appsettings.json` file.
> You can read the [official documentation](https://learn.microsoft.com/en-us/aspnet/core/security/app-secrets) for more information on how to do that in a .Net Core app.

And simply call `AddHoneybadger` without any parameters:

```c#
var builder = WebApplication.CreateBuilder(args);
builder.AddHoneybadger();
```

#### Usage

You can access the _Honeybadger Client_ using _DI_:

```c#
app.MapGet("/", ([FromServices] IHoneybadgerClient honeybadger) =>
{
    honeybadger.AddBreadcrumb("reached index route", "route", new Dictionary<string, object?>());

    return "Hello World!";
});
```

Any unhandled exceptions should be reported to Honeybadger automatically (unless `ReportUnhandledExceptions` is set to `false`):

```c#
app.MapGet("/debug", () =>
{
    throw new Exception("hello from .Net Core Web App!");
});
```

See example project in `examples/Honeybadger.DotNetCoreWebApp`.

### As a custom logging provider

#### 1. Install Honeybadger.Extensions.Logging from Nuget

```sh
dotnet add package Honeybadger.Extensions.Logging
```

#### 2. Register Honeybadger and additionally the custom logging provider:

```c#
var builder = WebApplication.CreateBuilder(args);
// or set the configuration in the appsettings.json file
builder.AddHoneybadger(configure =>
{
    configure.ApiKey = "{{PROJECT_API_KEY}}";
});
builder.Logging.AddHoneybadger();
```

You should also configure the minimum log level as you would configure other log providers in .Net Core.
The following would report only logged errors:

```json
{
   "Logging": {
      "Honeybadger": {
         "Default": "Error"
      }
   }
}
```

And simply call `AddHoneybadger` and `Logging.AddHoneybadger` without any parameters:

```c#
var builder = WebApplication.CreateBuilder(args);
builder.AddHoneybadger();
builder.Logging.AddHoneybadger();
```

#### Usage

Errors from the `logger` will be reported to Honeybadger:

```c#
app.MapGet("/notify", ([FromServices] ILogger<Program> logger) =>
{
      logger.LogError("hello from Honeybadger.Logger!");

      return "Log reported to Honeybadger. Check your dashboard!";
});
```

See example project in `examples/Honeybadger.DotNetCoreWebApp.Logger`.

### Send a test notification

> [!NOTE]
> Honeybadger, by default, will not report errors in development environments.
> You can override the development environments by setting the `DevelopmentEnvironments` property in the options.
> Alternatively, you can set the `ReportData` property to `true` to report errors in all environments.

You can send a test notification to Honeybadger to verify that the configuration is working.
Add the following to your `Program.cs` file:
```c#
// ...
builder.AddHoneybadger();
// ...
var app = builder.Build();
var honeybadger = app.Services.GetRequiredService<IHoneybadgerClient>();
await honeybadger.NotifyAsync("Hello from .Net!");
```

Run the app.
If the configuration is correctly set, you should see the notification in your Honeybadger dashboard.

### Automatic Error Reporting

Automatic error reporting is enabled by default, but you can disable it by setting
the `ReportUnhandledExceptions` property to `false` in `HoneybadgerOptions`:

```json
{
   "Honeybadger": {
      "ApiKey": "{{PROJECT_API_KEY}}",
      "AppEnvironment": "Development",
      "ReportData": true,
      "ReportUnhandledExceptions": false
   }
}
```

### Using the SDK manually

#### 1. Install the [Honeybadger Nuget](https://www.nuget.org/packages/Honeybadger).

```sh
dotnet add package Honeybadger
```

#### 2. Initialize the _Honeybadger Client_:

```c#
using Microsoft.Extensions.Options;

var options = new HoneybadgerOptions("{{PROJECT_API_KEY}}");
var honeybadger = new HoneybadgerClient(Options.Create(options));
```

#### 3. Call `notify` to report to Honeybadger:

```c#
// fire and forget
honeybadger.Notify("hello from .Net !");
// or async
await honeybadger.NotifyAsync("hello from .Net !");
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

All packages are published on nuget.org with a [Github Actions Workflow](./.github/workflows/release.yml).
The workflow does the following:

- `dotnet versionize` - bump versions and generate changelog
- `dotnet pack`
- `dotnet nuget push`

_Note: only users with write permissions can trigger this workflow (i.e. Collaborators)._

### Manual Releases

To release manually, execute the following steps:

1. Run `dotnet versionize` in the root directory. This will bump the version in all projects and commit the new version as a tag.
2. Run `dotnet pack --configuration Release -o ./artifacts`
3. Run `dotnet nuget push ./artifacts/*.nupkg --source https://api.nuget.org/v3/index.json --api-key YOUR_API_KEY`
4. Push the changes to the repository — at this point you will have pushed the git tags + the new versions of the packages to nuget.org.

## License

This Honeybadger repository and published packages are MIT licensed. See the [MIT-LICENSE](./MIT_LICENSE) file in this repository for details.
