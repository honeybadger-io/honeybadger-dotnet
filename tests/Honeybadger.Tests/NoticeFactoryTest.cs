using System;
using System.Collections.Generic;
using System.Linq;
using System.Text.Json;
using Honeybadger.NoticeHelpers;
using Honeybadger.Schema;
using Xunit;
using Xunit.Abstractions;

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
        var client = HoneybadgerSdk.Init(new HoneybadgerOptions {ReportData = true});
        client.AddBreadcrumb("a breadcrumb", "a category");
        var exception = new NamedException("exception");
        var notice = NoticeFactory.Make(client, exception);
        
        Console.WriteLine(JsonSerializer.Serialize(notice));
    
        Assert.NotNull(notice);
        Assert.NotNull(notice.Error);
        Assert.Equal("exception", notice.Error?.Message);
        Assert.Equal("Honeybadger.Tests.NamedException", notice.Error?.Class);
        Assert.True(notice.Breadcrumbs.Enabled);
        Assert.NotEmpty(notice.Breadcrumbs.Trail);
        AssertNotifier(notice);
    }

    [Fact]
    public void CreatesNotice_WithMaxBreadcrumbs()
    {
        var options = new HoneybadgerOptions {ReportData = true};
        var client = HoneybadgerSdk.Init(options);
        for (var i = 0; i < options.MaxBreadcrumbs + 2; i++)
        {
            client.AddBreadcrumb($"a breadcrumb {i}", "a category");
        }
    
        var exception = new NamedException("exception");
        var notice = NoticeFactory.Make(client, exception);
        
        Console.WriteLine(JsonSerializer.Serialize(notice));
    
        Assert.NotNull(notice);
        Assert.NotNull(notice.Error);
        Assert.Equal("exception", notice.Error?.Message);
        Assert.Equal("Honeybadger.Tests.NamedException", notice.Error?.Class);
        Assert.True(notice.Breadcrumbs.Enabled);
        Assert.NotEmpty(notice.Breadcrumbs.Trail);
        Assert.Equal(options.MaxBreadcrumbs, notice.Breadcrumbs.Trail.Length);
        var lastBreadcrumb = notice.Breadcrumbs.Trail.Last();
        Assert.Equal($"a breadcrumb {options.MaxBreadcrumbs + 1}", lastBreadcrumb.Message);
        AssertNotifier(notice);
    }

    [Fact]
    public void CreatesNotice_WithContext()
    {
        var client = HoneybadgerSdk.Init(new HoneybadgerOptions {ReportData = true});
        var noticeContext = new Dictionary<string, object>
        {
            {"user_id", 123}
        };
        var exception = new NamedException("exception");
        var notice = NoticeFactory.Make(client, exception, noticeContext);

        Assert.NotNull(notice);
        Assert.NotNull(notice.Error);
        Assert.Equal("exception", notice.Error?.Message);
        Assert.Equal("Honeybadger.Tests.NamedException", notice.Error?.Class);
        Assert.Equal(noticeContext, notice.Request.Context);
        AssertNotifier(notice);
    }

    private static void AssertNotifier(Notice notice)
    {
        Assert.NotNull(notice.Notifier);
        Assert.Equal(Constants.GithubUrl, notice.Notifier.Url);
    }
}