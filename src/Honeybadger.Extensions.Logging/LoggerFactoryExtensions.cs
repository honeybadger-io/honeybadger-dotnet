using Honeybadger;
using Honeybadger.Extensions.Logging;

// ReSharper disable once CheckNamespace
// Ensures 'AddHoneybadger' can be found without: 'using Honeybadger;'
namespace Microsoft.Extensions.Logging;

public static class LoggerFactoryExtensions
{
    public static ILoggerFactory AddHoneybadger(this ILoggerFactory factory, HoneybadgerLoggingOptions options)
    {
        var client = HoneybadgerSdk.Init(options);
        factory.AddProvider(new HoneybadgerLoggerProvider(client, options));
        
        return factory;
    }   
}