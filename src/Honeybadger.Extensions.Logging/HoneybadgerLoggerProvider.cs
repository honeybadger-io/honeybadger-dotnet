using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;

namespace Honeybadger.Extensions.Logging;

public class HoneybadgerLoggerProvider : ILoggerProvider
{
    private HoneybadgerLoggingOptions _currentConfig;
    private readonly IDisposable _onChangeToken;

    public HoneybadgerLoggerProvider(IOptionsMonitor<HoneybadgerLoggingOptions> config)
    {
        _currentConfig = config.CurrentValue;
        _onChangeToken = config.OnChange(updatedConfig => _currentConfig = updatedConfig);
    }
    
    public ILogger CreateLogger(string categoryName)
    {
        return new HoneybadgerLogger(() => _currentConfig);
    }
    
    public void Dispose()
    {
        _onChangeToken.Dispose();
    }
}