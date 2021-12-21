using System;
using Honeybadger.NoticeHelpers;
using Honeybadger.Schema;
using Xunit;

namespace Honeybadger.Tests;

public class NoticeFactoryTest
{
    [Fact]
    public void CreatesNotice_FromString()
    {
        var client = HoneybadgerSdk.Init(new HoneybadgerOptions());
        var notice = NoticeFactory.Make(client, "test");
        
        Assert.NotNull(notice);
        Assert.NotNull(notice.Error);
        Assert.Equal("test", notice.Error?.Message);
        Assert.Equal("Honeybadger.Tests.NoticeFactoryTest", notice.Error?.Class);
        AssertNotifier(notice);
    }

    [Fact]
    public void CreatesNotice_FromException()
    {
        var client = HoneybadgerSdk.Init(new HoneybadgerOptions());
        var exception = new NamedException("exception");
        var notice = NoticeFactory.Make(client, exception);

        Assert.NotNull(notice);
        Assert.NotNull(notice.Error);
        Assert.Equal("exception", notice.Error?.Message);
        Assert.Equal("Honeybadger.Tests.NamedException", notice.Error?.Class);
        AssertNotifier(notice);
    }

    [Fact]
    public void CreatesNotice_WithBreadcrumbs()
    {
        throw new NotImplementedException();
    }

    [Fact]
    public void CreatesNotice_WithMaxBreadcrumbs()
    {
        throw new NotImplementedException();
    }
    
    [Fact]
    public void CreatesNotice_WithContext()
    {
        throw new NotImplementedException();
    }

    private static void AssertNotifier(Notice notice)
    {
        Assert.NotNull(notice.Notifier);
        Assert.Equal(Constants.GithubUrl, notice.Notifier.Url);
    }
}