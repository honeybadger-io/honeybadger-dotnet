using System;
using System.Reflection;
using Honeybadger.Schema;
using Xunit;

namespace Honeybadger.Tests;

public class NoticeFactoryTest
{
    [Fact]
    public void CreatesNotice_FromString()
    {
        var notice = NoticeFactory.Make("test");
        
        Assert.NotNull(notice);
        Assert.NotNull(notice.Error);
        Assert.Equal("test", notice.Error?.Message);
        Assert.Equal("Honeybadger.Tests.NoticeFactoryTest", notice.Error?.Class);
    }
    
    [Fact]
    public void CreatesNotice_FromException()
    {
        var exception = new NamedException("exception");
        var notice = NoticeFactory.Make(exception);
        
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