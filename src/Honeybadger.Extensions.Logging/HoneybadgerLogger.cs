using Microsoft.Extensions.Logging;

namespace Honeybadger.Extensions.Logging;

public class HoneybadgerLogger : ILogger
{
    private IHoneybadgerClient? _client = null;
    private readonly Func<HoneybadgerLoggingOptions> _getOptions;

    internal HoneybadgerLogger(Func<HoneybadgerLoggingOptions> getCurrentConfig)
    {
        _getOptions = getCurrentConfig;
    }

    public void Log<TState>(LogLevel logLevel, EventId eventId, TState state, Exception? exception,
        Func<TState, Exception?, string> formatter)
    {
        if (_client == null)
        {
            _client = HoneybadgerSdk.Init(_getOptions());
        }
        
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
        
        if (ShouldReport(logLevel) && exception != null)
        {
            _client.Notify(exception);
        }
    }

    public bool IsEnabled(LogLevel logLevel)
    {
        return _getOptions().ReportData;
    }

    public IDisposable BeginScope<TState>(TState state) => default;

    private bool ShouldReport(LogLevel logLevel)
    {
        // todo: need to check more options here
        return _getOptions().MinimumNoticeLevel >= logLevel;
    }

    private bool ShouldAddBreadcrumb(LogLevel logLevel)
    {
        return _getOptions().MinimumBreadcrumbLevel >= logLevel;
    }
}