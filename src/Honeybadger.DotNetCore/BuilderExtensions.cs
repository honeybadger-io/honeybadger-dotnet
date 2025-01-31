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
    public static void AddHoneybadger(this IHostApplicationBuilder builder, Action<HoneybadgerOptions>? configure = null)
    {
        //services.TryAddSingleton<IHttpContextAccessor, HttpContextAccessor>();
        
        var configSection = builder.Configuration.GetSection("Honeybadger");
        
        if (configure is not null)
        {
            var options = new HoneybadgerOptions();
            configSection.Bind(options);            
            configure(options);
            builder.Services.Configure<HoneybadgerOptions>(config =>
            {
                config.ApiKey = options.ApiKey;
                config.AppEnvironment = options.AppEnvironment;
                config.BreadcrumbsEnabled = options.BreadcrumbsEnabled;
                config.DevelopmentEnvironments = options.DevelopmentEnvironments;
                config.Endpoint = options.Endpoint;
                config.FilterKeys = options.FilterKeys;
                config.HostName = options.HostName;
                config.HttpClient = options.HttpClient;
                config.MaxBreadcrumbs = options.MaxBreadcrumbs;
                config.ProjectRoot = options.ProjectRoot;
                config.ReportData = options.ReportData;
                config.Revision = options.Revision;
                config.ReportUnhandledExceptions = options.ReportUnhandledExceptions;
                
            });
        }
        else
        {
            builder.Services.Configure<HoneybadgerOptions>(configSection);
        }
        
        builder.Services
            .AddHttpClient()
            .AddSingleton<IStartupFilter, HoneybadgerStartupFilter>()
            .AddScoped<IHoneybadgerClient, HoneybadgerClient>();
    }
}