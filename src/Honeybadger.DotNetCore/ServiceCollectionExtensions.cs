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
            //.AddSingleton<IStartupFilter, BugsnagStartupFilter>()
            .AddScoped<IHoneybadgerClient, HoneybadgerClient>(context => (HoneybadgerClient) HoneybadgerSdk.Init(options));
    }
}