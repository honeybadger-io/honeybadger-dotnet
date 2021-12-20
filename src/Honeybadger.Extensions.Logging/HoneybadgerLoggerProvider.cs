using Microsoft.Extensions.Logging;

namespace Honeybadger.Extensions.Logging;

public class HoneybadgerLoggerProvider : ILoggerProvider
{
    private IHoneybadgerClient _client;
    private HoneybadgerLoggingOptions _options;

    public HoneybadgerLoggerProvider(IHoneybadgerClient client, HoneybadgerLoggingOptions options)
    {
        _client = client;
        _options = options;
    }

    public void Dispose()
    {
        throw new NotImplementedException();
    }

    public ILogger CreateLogger(string categoryName)
    {
        throw new NotImplementedException();
    }
}