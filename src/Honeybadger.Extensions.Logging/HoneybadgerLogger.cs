using Microsoft.Extensions.Logging;

namespace Honeybadger.Extensions.Logging;

public class HoneybadgerLogger : ILogger
{
    private readonly IHoneybadgerClient _client;
    private readonly Func<HoneybadgerLoggerOptions> _getOptions;

    internal HoneybadgerLogger(IHoneybadgerClient client, Func<HoneybadgerLoggerOptions> getCurrentConfig)
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
        
        if (exception is null)
        {
            var notice = formatter(state, exception);
            _client.NotifyAsync(notice).ConfigureAwait(false);
        }
        // We might reach here because of logger.UnhandledException.
        // This may be called from DiagnosticsLoggerExtensions.cs (DeveloperExceptionPageMiddleware.cs)
        // If ReportUnhandledExceptions is set to false, we should not report the exception.
        else if (ShouldReportException(eventId))
        {
            _client.NotifyAsync(exception).ConfigureAwait(false);
        }
    }

    public bool IsEnabled(LogLevel logLevel)
    {
        return _client.Options.ShouldReport();
    }
    
    private bool IsUnhandledException(EventId eventId)
    {
        return eventId.Name?.Equals("UnhandledException") == true;
    }
    
    private bool ShouldReportException(EventId eventId)
    {
        return !IsUnhandledException(eventId) ||
               (IsUnhandledException(eventId) && _client.Options.ReportUnhandledExceptions);
    }

    public IDisposable BeginScope<TState>(TState state) where TState : notnull => null!;
}