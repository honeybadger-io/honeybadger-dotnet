using System.Runtime.Versioning;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;

namespace Honeybadger.Extensions.Logging;

[UnsupportedOSPlatform("browser")]
[ProviderAlias("Honeybadger")]
public class HoneybadgerLoggerProvider : ILoggerProvider
{
    private readonly IHoneybadgerClient _client;
    private readonly IDisposable _onChangeToken;
    private HoneybadgerLoggingOptions _currentConfig;

    public HoneybadgerLoggerProvider(IHoneybadgerClient client, IOptionsMonitor<HoneybadgerLoggingOptions> config)
    {
        _client = client;
        _currentConfig = config.CurrentValue;
        _onChangeToken = config.OnChange(updatedConfig => _currentConfig = updatedConfig)!;
    }
    
    public ILogger CreateLogger(string categoryName)
    {
        _client.Configure(_currentConfig);
        return new HoneybadgerLogger(_client, () => _currentConfig);
    }
    
    public void Dispose()
    {
        _onChangeToken.Dispose();
    }
}