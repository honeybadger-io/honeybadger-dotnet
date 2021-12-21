using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.DependencyInjection;

namespace Honeybadger.DotNetCore;

public static class ServiceCollectionExtensions
{
    /// <summary>
    /// Adds Honeybadger client.
    /// Registers a middleware to report unhandled exceptions.
    /// </summary>
    public static IServiceCollection AddHoneybadger(this IServiceCollection services, HoneybadgerOptions options)
    {
        //services.TryAddSingleton<IHttpContextAccessor, HttpContextAccessor>();

        return services
            .AddSingleton<IStartupFilter, HoneybadgerStartupFilter>()
            .AddScoped<IHoneybadgerClient, HoneybadgerClient>(context => (HoneybadgerClient) HoneybadgerSdk.Init(options));
    }
    
    /// <summary>
    /// Adds Honeybadger client.
    /// Registers a middleware to report unhandled exceptions.
    /// </summary>
    public static IServiceCollection AddHoneybadger(this IServiceCollection services)
    {
        return services.AddHoneybadger(new HoneybadgerOptions());
    }
}