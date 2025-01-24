using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;

namespace Honeybadger.DotNetCore;

public static class ServiceCollectionExtensions
{
    /// <summary>
    /// Adds Honeybadger client.
    /// Registers a middleware to report unhandled exceptions.
    /// </summary>
    public static void AddHoneybadger(this IHostApplicationBuilder builder, HoneybadgerOptions options)
    {
        //services.TryAddSingleton<IHttpContextAccessor, HttpContextAccessor>();

        builder.Services.Configure<HoneybadgerOptions>(config =>
        {
            config.ApiKey = options.ApiKey;
            config.AppEnvironment = options.AppEnvironment;
            config.BreadcrumbsEnabled = options.BreadcrumbsEnabled;
            config.DevelopmentEnvironments = options.DevelopmentEnvironments;
            config.Endpoint = options.Endpoint;
            config.Revision = options.Revision;
            config.FilterKeys = options.FilterKeys;
            config.HostName = options.HostName;
            config.MaxBreadcrumbs = options.MaxBreadcrumbs;
            config.ProjectRoot = options.ProjectRoot;
            config.ReportData = options.ReportData;
        });

        builder.Services
            .AddHttpClient()
            .AddSingleton<IStartupFilter, HoneybadgerStartupFilter>()
            .AddScoped<IHoneybadgerClient, HoneybadgerClient>();
    }
    
    /// <summary>
    /// Adds Honeybadger client.
    /// Registers a middleware to report unhandled exceptions.
    /// </summary>
    public static void AddHoneybadger(this IHostApplicationBuilder builder)
    {
        var options = new HoneybadgerOptions();
        builder.Configuration.GetSection("Honeybadger").Bind(options);
        builder.AddHoneybadger(options);
    }
}