# Honeybadger for .Net Apps

This is the .Net Honeybadger Notifier.

## Supported .Net versions:

All modern .Net Core applications are supported, up to .Net 9.0.

## Getting Started

### Configuration

The Honeybadger Notifier can be configured using the `HoneybadgerOptions` class.
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
   builder.AddHoneybadger(new HoneybadgerOptions("apiKey"));
   ```
   
   Or you can configure Honeybadger through your `appsettings.json` file, by adding a `Honeybadger` section:
   ```json
   {
     "Honeybadger": {
       "ApiKey": "apiKey",
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

You can access the _Honeybadger Client_ using _DI_:
```c#
app.MapGet("/", ([FromServices] IHoneybadgerClient client) =>
{
    client.AddBreadcrumb("reached index route", "route", new Dictionary<string, object?>());
    
    return "Hello World!";
});
```

Any unhandled exceptions should be reported to Honeybadger automatically (unless `ReportUnhandledExceptions` is set to false):
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
2. Register Honeybadger and additionally the custom logging provider:
   ```c#
   var builder = WebApplication.CreateBuilder(args);
   // or set the configuration in the appsettings.json file
   builder.AddHoneybadger(new HoneybadgerOptions("apiKey"));
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
   Note: If you want to disable automatic reporting of unhandled exceptions, you can set the `ReportUnhandledExceptions` property to `false` in the `HoneybadgerOptions`:
   ```json
   {
     "Honeybadger": {
       "ApiKey": "apiKey",
       "AppEnvironment": "Development",
       "ReportData": true,
       "ReportUnhandledExceptions": false
     }
   }
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
   // blocking
   client.Notify("hello from .Net !");
   // or async
   await client.NotifyAsync("hello from .Net !");
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

**Note**: Automated releases are not yet fully functional. See [Manual Releases](#manual-releases) for the current process.

All packages are published on nuget.org with a [Github Actions Worfklow](./.github/workflows/release.yml).
The workflow does the following:
- `dotnet versionize` - bump versions and generate changelog
- `dotnet pack`
- `dotnet package push`

_Note: only users with write permissions can trigger this workflow (i.e. Collaborators)._

### Manual Releases

Our automated release process is not yet fully functional for the following reason:  
Since `Honeybadger` is a dependency of `Honeybadger.Extensions.Logging` and `Honeybadger.DotNetCore`, 
we need to release `Honeybadger` first, then update the dependencies in the other projects and finally release those two projects.

To release manually, execute the following steps:
1. Run `dotnet versionize` in the root directory. This will bump the version in all projects and commit the new version as a tag.
2. Run `dotnet pack ./src/Honeybadger --configuration Release`
3. Run `dotnet nuget push ./src/Honeybadger/bin/Release/*.nupkg --source https://api.nuget.org/v3/index.json --api-key YOUR_API_KEY`
4. Update the dependencies in `Honeybadger.Extensions.Logging` and `Honeybadger.DotNetCore` to the new version of `Honeybadger`.
5. Run `dotnet pack ./src/Honeybadger.Extensions.Logging --configuration Release`
6. Run `dotnet nuget push ./src/Honeybadger.Extensions.Logging/bin/Release/*.nupkg --source https://api.nuget.org/v3/index.json --api-key YOUR_API_KEY`
7. Run `dotnet pack ./src/Honeybadger.DotNetCore --configuration Release`
8. Run `dotnet nuget push ./src/Honeybadger.DotNetCore/bin/Release/*.nupkg --source https://api.nuget.org/v3/index.json --api-key YOUR_API_KEY`
9. Commit the changes in the `Honeybadger.Extensions.Logging` and `Honeybadger.DotNetCore` projects.
10. Push the changes to the repository - at this point you will have pushed the git tags + the new versions of the packages to nuget.org.

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
