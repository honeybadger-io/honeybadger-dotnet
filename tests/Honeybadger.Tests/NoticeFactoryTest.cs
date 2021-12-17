using System;
using Honeybadger.NoticeHelpers;
using Xunit;

namespace Honeybadger.Tests;

public class NoticeFactoryTest
{
    [Fact]
    public void CreatesNotice_FromString()
    {
        var client = new NullClient();
        var notice = NoticeFactory.Make(client, "test");
        
        Assert.NotNull(notice);
        Assert.NotNull(notice.Error);
        Assert.Equal("test", notice.Error?.Message);
        Assert.Equal("Honeybadger.Tests.NoticeFactoryTest", notice.Error?.Class);
    }
    
    [Fact]
    public void CreatesNotice_FromException()
    {
        var client = new NullClient();
        var exception = new NamedException("exception");
        var notice = NoticeFactory.Make(client, exception);
        
        Assert.NotNull(notice);
        Assert.NotNull(notice.Error);
        Assert.Equal("exception", notice.Error?.Message);
        Assert.Equal("Honeybadger.Tests.NamedException", notice.Error?.Class);
    }
    
    [Fact]
    public void CreatesNotice_FromAnonymousObject()
    {
        throw new NotImplementedException();
    }
    
    [Fact]
    public void CreatesNotice_WithDetails()
    {
        throw new NotImplementedException();
    }
    
    [Fact]
    public void CreatesNotice_WithNotifier()
    {
        throw new NotImplementedException();
    }
    
    [Fact]
    public void CreatesNotice_WithRequestAndServer()
    {
        throw new NotImplementedException();
    }
    
    [Fact]
    public void CreatesNotice_WithError()
    {
        throw new NotImplementedException();
    }

}