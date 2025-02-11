using Honeybadger;
using Honeybadger.Extensions.Logging;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.DependencyInjection.Extensions;
using Microsoft.Extensions.Logging.Configuration;

// ReSharper disable once CheckNamespace
// Ensures 'AddHoneybadger' can be found without: 'using Honeybadger;'
namespace Microsoft.Extensions.Logging;

public static class LoggerFactoryExtensions
{
    public static ILoggingBuilder AddHoneybadger(
        this ILoggingBuilder builder)
    {
        builder.AddConfiguration();

        builder.Services.AddSingleton<IHoneybadgerClient, HoneybadgerClient>();
        
        builder.Services.TryAddEnumerable(
            ServiceDescriptor.Singleton<ILoggerProvider, HoneybadgerLoggerProvider>());
        
        LoggerProviderOptions.RegisterProviderOptions
            <HoneybadgerLoggerOptions, HoneybadgerLoggerProvider>(builder.Services);

        return builder;
    }

    public static ILoggingBuilder AddHoneybadger(this ILoggingBuilder builder, Action<HoneybadgerLoggerOptions> configure)
    {
        builder.AddHoneybadger();
        builder.Services.Configure(configure);

        return builder;
    }
}