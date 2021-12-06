using Honeybadger.Schema;
using Microsoft.Extensions.Logging;

namespace Honeybadger.Extensions.Logging;

public class HoneybadgerLogger : ILogger
{
    private readonly IHoneybadgerClient _client;
    private readonly HoneybadgerOptions _options;

    internal HoneybadgerLogger(IHoneybadgerClient client, HoneybadgerOptions options)
    {
        _client = client;
        _options = options;
    }

    public void Log<TState>(LogLevel logLevel, EventId eventId, TState state, Exception? exception, Func<TState, Exception?, string> formatter)
    {
        if (!IsEnabled(logLevel))
        {
            return;
        }

        if (exception == null)
        {
            return;
        }
        
        var notice = NoticeFactory.Make(exception);
        _client.Notify(notice);
    }

    public bool IsEnabled(LogLevel logLevel)
    {
        // todo: need to check more options here
        return !_options.IsDisabled && logLevel != LogLevel.None;
    }

    public IDisposable BeginScope<TState>(TState state)
    {
        throw new NotImplementedException();
    }
}