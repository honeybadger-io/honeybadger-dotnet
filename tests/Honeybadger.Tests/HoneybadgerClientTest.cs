using Xunit;

namespace Honeybadger.Tests;

public class HoneybadgerClientTest
{
    [Fact]
    public void InitializesClient()
    {
        var client = HoneybadgerSdk.Init(new HoneybadgerOptions
        {
            IsDisabled = false
        });
        Assert.True(client is HoneybadgerClient);
    }

    [Fact]
    public void ReturnsNullClientIfDisabled()
    {
        var client = HoneybadgerSdk.Init(new HoneybadgerOptions
        {
            IsDisabled = true
        });
        Assert.True(client is NullClient);
    }
    
}