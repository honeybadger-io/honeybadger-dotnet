using Microsoft.Extensions.Logging;

namespace Honeybadger.Extensions.Logging;

public class HoneybadgerLogger : ILogger
{
    private readonly IHoneybadgerClient _client;
    private readonly Func<HoneybadgerLoggingOptions> _getOptions;

    internal HoneybadgerLogger(IHoneybadgerClient client, Func<HoneybadgerLoggingOptions> getCurrentConfig)
    {
        _client = client;
        _getOptions = getCurrentConfig;
    }

    public void Log<TState>(LogLevel logLevel, EventId eventId, TState state, Exception? exception,
        Func<TState, Exception?, string> formatter)
    {
        if (!IsEnabled(logLevel))
        {
            return;
        }

        if (ShouldAddBreadcrumb(logLevel))
        {
            var message = formatter(state, exception);
            _client.AddBreadcrumb(
                message,
                "log",
                new Dictionary<string, object?>()
                {
                    {"Level", logLevel},
                }
            );    
        }
        
        if (ShouldReport(logLevel))
        {
            if (exception is null)
            {
                var notice = formatter(state, exception);
                _client.Notify(notice);
            }
            else
            {
                _client.Notify(exception);
            }
        }
    }

    public bool IsEnabled(LogLevel logLevel)
    {
        return _getOptions().ShouldReport();
    }

    public IDisposable BeginScope<TState>(TState state) where TState : notnull => null!;

    private bool ShouldReport(LogLevel logLevel)
    {
        return _getOptions().MinimumNoticeLevel <= logLevel;
    }

    private bool ShouldAddBreadcrumb(LogLevel logLevel)
    {
        return _getOptions().MinimumBreadcrumbLevel >= logLevel;
    }
}