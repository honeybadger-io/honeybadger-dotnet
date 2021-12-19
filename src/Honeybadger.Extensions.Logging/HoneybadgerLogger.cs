using Microsoft.Extensions.Logging;

namespace Honeybadger.Extensions.Logging;

public class HoneybadgerLogger : ILogger
{
    private readonly IHoneybadgerClient _client;
    private readonly HoneybadgerOptions _options;

    internal HoneybadgerLogger(HoneybadgerOptions options)
    {
        _client = HoneybadgerSdk.Init(options);
        _options = options;
    }

    public void Log<TState>(LogLevel logLevel, EventId eventId, TState state, Exception? exception,
        Func<TState, Exception?, string> formatter)
    {
        if (!IsEnabled(logLevel))
        {
            return;
        }

        var message = formatter(state, exception);
        _client.AddBreadcrumb(
            message,
            "log",
            new Dictionary<string, object?>()
            {
                {"Level", logLevel},
            }
        );

        if (ShouldReport(logLevel) && exception != null)
        {
            _client.Notify(exception);
        }
    }

    public bool IsEnabled(LogLevel logLevel)
    {
        return _client is not NullClient;
    }

    private bool ShouldReport(LogLevel logLevel)
    {
        // todo: need to check more options here
        return _options.ReportData && logLevel is LogLevel.Error or LogLevel.Critical;
    }

    public IDisposable BeginScope<TState>(TState state)
    {
        throw new NotImplementedException();
    }
}