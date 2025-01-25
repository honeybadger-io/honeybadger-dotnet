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
        
        builder.Services.Configure<HoneybadgerOptions>(builder.Configuration.GetSection("Honeybadger"));

        if (configure is not null)
        {
            builder.Services.Configure(configure);    
        }
        
        builder.Services
            .AddHttpClient()
            .AddSingleton<IStartupFilter, HoneybadgerStartupFilter>()
            .AddScoped<IHoneybadgerClient, HoneybadgerClient>();
    }
}