using Xunit;

namespace Honeybadger.Tests;

public class HoneybadgerClientTest
{
    [Fact]
    public void InitializesClient()
    {
        var client = HoneybadgerSdk.Init(new HoneybadgerOptions("test")
        {
             ReportData = false
        });
        Assert.True(client is HoneybadgerClient);
    }

    [Fact]
    public void NullClient_IfDisabled()
    {
        var client = HoneybadgerSdk.Init(new HoneybadgerOptions("test")
        {
            ReportData = false
        });
        Assert.True(client is NullClient);
    }

    [Fact]
    public void SendsNotice_ResponseOk()
    {
    }

    [Fact]
    public void SendsNotice_ResponseNotOk()
    {
        
    }

}