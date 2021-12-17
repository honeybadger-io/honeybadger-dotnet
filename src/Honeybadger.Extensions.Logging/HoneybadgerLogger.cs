using Honeybadger.NoticeHelpers;
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

    public void Log<TState>(LogLevel logLevel, EventId eventId, TState state, Exception? exception, Func<TState, Exception?, string> formatter)
    {
        if (!IsEnabled(logLevel) || exception == null)
        {
            return;
        }

        var notice = NoticeFactory.Make(_client, exception);
        _client.Notify(notice);
    }

    public bool IsEnabled(LogLevel logLevel)
    {
        if (_client is NullClient)
        {
            return false;
        }
        
        // todo: need to check more options here
        return _options.ReportData && logLevel is LogLevel.Error or LogLevel.Critical;
    }

    public IDisposable BeginScope<TState>(TState state)
    {
        throw new NotImplementedException();
    }
}