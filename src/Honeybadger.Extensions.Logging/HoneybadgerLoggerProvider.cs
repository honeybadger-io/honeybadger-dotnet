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
    private HoneybadgerLoggerOptions _currentConfig;

    public HoneybadgerLoggerProvider(IHoneybadgerClient client, IOptionsMonitor<HoneybadgerLoggerOptions> config)
    {
        _client = client;
        _currentConfig = config.CurrentValue;
        _onChangeToken = config.OnChange(updatedConfig => _currentConfig = updatedConfig)!;
    }
    
    public ILogger CreateLogger(string categoryName)
    {
        return new HoneybadgerLogger(_client, () => _currentConfig);
    }
    
    public void Dispose()
    {
        _onChangeToken.Dispose();
    }
}