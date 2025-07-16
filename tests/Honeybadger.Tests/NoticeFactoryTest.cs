using System.Collections.Generic;
using System.Linq;
using Honeybadger.Schema;
using Microsoft.Extensions.Options;
using Xunit;

namespace Honeybadger.Tests;

public class NoticeFactoryTest
{
    [Fact]
    public void CreatesNotice_FromString()
    {
        var noticeFactory = new BaseNoticeFactory();
        var client = new HoneybadgerClient(Options.Create(new HoneybadgerOptions()), noticeFactory);
        // We are passing framesToSkip because we don't want to skip "Honeybadger" frames
        // (the test project is called Honeybadger.Tests).
        // We set framesToSkip to 1 to skip the NoticeFactory.Make method.
        var notice = noticeFactory.Make(client, "test", framesToSkip: 1);

        Assert.NotNull(notice);
        Assert.NotNull(notice.Error);
        Assert.Equal("test", notice.Error.Message);
        Assert.Equal("Honeybadger.Tests.NoticeFactoryTest", notice.Error.Class);
        Assert.Equal("CreatesNotice_FromString", notice.Error.Backtrace[0].Method);
        AssertNotifier(notice);
    }

    [Fact]
    public void CreatesNotice_FromException()
    {
        var noticeFactory = new BaseNoticeFactory();
        var client = new HoneybadgerClient(Options.Create(new HoneybadgerOptions()), noticeFactory);
        var exception = new NamedException("exception");
        var notice = noticeFactory.Make(client, exception);

        Assert.NotNull(notice);
        Assert.NotNull(notice.Error);
        Assert.Equal("exception", notice.Error.Message);
        Assert.Equal("Honeybadger.Tests.NamedException", notice.Error.Class);
        Assert.Equal("CreatesNotice_FromException", notice.Error.Backtrace[0].Method);
        AssertNotifier(notice);
    }

    [Fact]
    public void CreatesNotice_WithBreadcrumbs()
    {
        var noticeFactory = new BaseNoticeFactory();
        var client = new HoneybadgerClient(Options.Create(new HoneybadgerOptions
        {
            ApiKey = "test",
            ReportData = true
        }), noticeFactory);
        client.AddBreadcrumb("a breadcrumb", "a category");
        var exception = new NamedException("exception");
        var notice = noticeFactory.Make(client, exception);
        
        Assert.NotNull(notice);
        Assert.NotNull(notice.Error);
        Assert.Equal("exception", notice.Error.Message);
        Assert.Equal("Honeybadger.Tests.NamedException", notice.Error.Class);
        Assert.Equal("CreatesNotice_WithBreadcrumbs", notice.Error.Backtrace[0].Method);
        Assert.True(notice.Breadcrumbs.Enabled);
        if (notice.Breadcrumbs.Trail.Length == 0)
        {
            // Console.WriteLine("CreatesNotice_WithBreadcrumbs: no breadcrumbs");
        }
        Assert.NotEmpty(notice.Breadcrumbs.Trail);
        AssertNotifier(notice);
    }

    [Fact]
    public void CreatesNotice_WithMaxBreadcrumbs()
    {
        var options = new HoneybadgerOptions
        {
            ApiKey = "test",
            ReportData = true
        };
        var noticeFactory = new BaseNoticeFactory();
        var client = new HoneybadgerClient(Options.Create(options), noticeFactory);
        for (var i = 0; i < options.MaxBreadcrumbs + 2; i++)
        {
            client.AddBreadcrumb($"a breadcrumb {i}", "a category");
        }
    
        var exception = new NamedException("exception");
        var notice = noticeFactory.Make(client, exception);
        
        Assert.NotNull(notice);
        Assert.NotNull(notice.Error);
        Assert.Equal("exception", notice.Error.Message);
        Assert.Equal("Honeybadger.Tests.NamedException", notice.Error.Class);
        Assert.Equal("CreatesNotice_WithMaxBreadcrumbs", notice.Error.Backtrace[0].Method);
        Assert.True(notice.Breadcrumbs.Enabled);
        if (!notice.Breadcrumbs.Trail.Any())
        {
            // Console.WriteLine("CreatesNotice_WithBreadcrumbs: no breadcrumbs");
        }
        Assert.NotEmpty(notice.Breadcrumbs.Trail);
        Assert.Equal(options.MaxBreadcrumbs, notice.Breadcrumbs.Trail.Length);
        var lastBreadcrumb = notice.Breadcrumbs.Trail.Last();
        Assert.Equal($"a breadcrumb {options.MaxBreadcrumbs + 1}", lastBreadcrumb.Message);
        AssertNotifier(notice);
    }

    [Fact]
    public void CreatesNotice_WithContext()
    {
        var noticeFactory = new BaseNoticeFactory();
        var client = new HoneybadgerClient(Options.Create(new HoneybadgerOptions { ReportData = true }), noticeFactory);
        var noticeContext = new Dictionary<string, object>
        {
            {"user_id", 123}
        };
        var exception = new NamedException("exception");
        var notice = noticeFactory.Make(client, exception, noticeContext);

        Assert.NotNull(notice);
        Assert.NotNull(notice.Error);
        Assert.Equal("exception", notice.Error.Message);
        Assert.Equal("Honeybadger.Tests.NamedException", notice.Error.Class);
        Assert.Equal("CreatesNotice_WithContext", notice.Error.Backtrace[0].Method);
        Assert.Equal(noticeContext, notice.Request.Context);
        AssertNotifier(notice);
    }

    [Fact]
    public void CreatesNotice_WithContextInStackFrame()
    {
        var noticeFactory = new BaseNoticeFactory();
        var client = new HoneybadgerClient(Options.Create(new HoneybadgerOptions()), noticeFactory);
        // We are passing framesToSkip because we don't want to skip "Honeybadger" frames
        // (the test project is called Honeybadger.Tests).
        // We set framesToSkip to 1 to skip the NoticeFactory.Make method.
        var notice = noticeFactory.Make(client, "test", framesToSkip: 1);

        Assert.NotNull(notice);
        Assert.NotNull(notice.Error);
        Assert.Equal("test", notice.Error.Message);
        Assert.Equal("Honeybadger.Tests.NoticeFactoryTest", notice.Error.Class);
        Assert.Equal("CreatesNotice_WithContextInStackFrame", notice.Error.Backtrace[0].Method);
        Assert.Equal(Context.App, notice.Error.Backtrace[0].Context);
        Assert.Equal(Context.All, notice.Error.Backtrace[1].Context);
        AssertNotifier(notice);
    }

    private static void AssertNotifier(Notice notice)
    {
        Assert.NotNull(notice.Notifier);
        Assert.Equal(Constants.GithubUrl, notice.Notifier.Url);
    }
}