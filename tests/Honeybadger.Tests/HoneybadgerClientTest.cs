using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Text.Json;
using System.Threading;
using System.Threading.Tasks;
using Honeybadger.Schema;
using Microsoft.Extensions.Options;
using Moq;
using Moq.Protected;
using Xunit;

namespace Honeybadger.Tests;

public class HoneybadgerClientTest
{
    [Fact]
    public void InitializesClient()
    {
        var options = Options.Create(new HoneybadgerOptions
        {
            ApiKey = "test",
            ReportData = false
        });
        
        var client = new HoneybadgerClient(options);
        Assert.NotNull(client);
    }
    
    [Fact]
    public async Task SendsNotice_WithBreadcrumbs()
    {
        const string noticeMessage = "notice with breadcrumbs";
        const string breadcrumbMessage = "breadcrumb";
        const string breadcrumbCategory = "test";
        var breadcrumbMetadata = new Dictionary<string, object?>()
        {
            {"metadata", "value"}
        };
        var noticeReceived = new TaskCompletionSource();
        var mockHttpHandler = new Mock<HttpMessageHandler>();
        mockHttpHandler.Protected()
            .Setup<Task<HttpResponseMessage>>("SendAsync", ItExpr.IsAny<HttpRequestMessage>(), ItExpr.IsAny<CancellationToken>())
            .Callback((HttpRequestMessage request, CancellationToken token) =>
            {
                var content = request.Content?.ReadAsStringAsync(token).Result;
                var notice = JsonSerializer.Deserialize<Notice>(content!)!;
                Assert.NotNull(notice);
                Assert.Equal(noticeMessage, notice.Error.Message);
                Assert.NotNull(notice.Breadcrumbs);
                Assert.True(notice.Breadcrumbs.Enabled);
                Assert.NotEmpty(notice.Breadcrumbs.Trail);
                var firstTrail = notice.Breadcrumbs.Trail[0];
                Assert.Equal(breadcrumbMessage, firstTrail.Message);
                Assert.Equal(breadcrumbCategory, firstTrail.Category);
                Assert.Equal(breadcrumbMetadata.Count, firstTrail.Metadata?.Count);
                Assert.Equal(breadcrumbMetadata.First().Key, firstTrail.Metadata?.First().Key);
                Assert.Equal(breadcrumbMetadata.First().Value, firstTrail.Metadata?.First().Value?.ToString());
                noticeReceived.SetResult();
            })
            .ReturnsAsync(new HttpResponseMessage {
                StatusCode = HttpStatusCode.OK,
                Content = new StringContent("")
            });
        
        var options = Options.Create(new HoneybadgerOptions
        {
            ApiKey = "test",
            HttpClient = new HttpClient(mockHttpHandler.Object) { BaseAddress = new Uri("https://notavaliddomain.com") }
        });
        var client = new HoneybadgerClient(options);
        client.AddBreadcrumb(breadcrumbMessage, breadcrumbCategory, breadcrumbMetadata);
        // ReSharper disable once MethodHasAsyncOverload
        client.Notify(noticeMessage);
        await noticeReceived.Task;
    }
    
    [Fact]
    public async Task SendsNotice_WithContext()
    {
        const string noticeMessage = "notice with context";
        var noticeContext = new Dictionary<string, object>()
        {
            {"user_id", 123}
        };
        var noticeReceived = new TaskCompletionSource();
        var mockHttpHandler = new Mock<HttpMessageHandler>();
        mockHttpHandler.Protected()
            .Setup<Task<HttpResponseMessage>>("SendAsync", ItExpr.IsAny<HttpRequestMessage>(), ItExpr.IsAny<CancellationToken>())
            .Callback((HttpRequestMessage request, CancellationToken token) =>
            {
                var content = request.Content?.ReadAsStringAsync(token).Result;
                var notice = JsonSerializer.Deserialize<Notice>(content!)!;
                Assert.NotNull(notice);
                Assert.Equal(noticeMessage, notice.Error.Message);
                Assert.NotNull(notice.Request);
                Assert.Equal(noticeContext.Count, notice.Request.Context?.Count);
                Assert.Equal(noticeContext.First().Key, notice.Request.Context?.First().Key);
                Assert.Equal(noticeContext.First().Value.ToString(), notice.Request.Context?.First().Value.ToString());
                noticeReceived.SetResult();
            })
            .ReturnsAsync(new HttpResponseMessage {
                StatusCode = HttpStatusCode.OK,
                Content = new StringContent("")
            });
        
        var options = Options.Create(new HoneybadgerOptions
        {
            ApiKey = "test",
            HttpClient = new HttpClient(mockHttpHandler.Object) { BaseAddress = new Uri("https://notavaliddomain.com") }
        });
        var client = new HoneybadgerClient(options);
        client.AddContext(noticeContext);
        // ReSharper disable once MethodHasAsyncOverload
        client.Notify(noticeMessage);
        await noticeReceived.Task;
    }

    [Fact]
    public async Task SendsNotice_WithTags()
    {
        const string noticeMessage = "notice with tags";
        string[] tags = ["v1.0.0"];
        var noticeReceived = new TaskCompletionSource();
        var mockHttpHandler = new Mock<HttpMessageHandler>();
        mockHttpHandler.Protected()
            .Setup<Task<HttpResponseMessage>>("SendAsync", ItExpr.IsAny<HttpRequestMessage>(), ItExpr.IsAny<CancellationToken>())
            .Callback((HttpRequestMessage request, CancellationToken token) =>
            {
                var content = request.Content?.ReadAsStringAsync(token).Result;
                var notice = JsonSerializer.Deserialize<Notice>(content!)!;
                Assert.NotNull(notice);
                Assert.Equal(noticeMessage, notice.Error.Message);
                Assert.NotNull(notice.Request);
                Assert.Equal(tags, notice.Error.Tags);
                noticeReceived.SetResult();
            })
            .ReturnsAsync(new HttpResponseMessage {
                StatusCode = HttpStatusCode.OK,
                Content = new StringContent("")
            });
        
        var options = Options.Create(new HoneybadgerOptions
        {
            ApiKey = "test",
            HttpClient = new HttpClient(mockHttpHandler.Object) { BaseAddress = new Uri("https://notavaliddomain.com") }
        });
        var client = new HoneybadgerClient(options);
        // ReSharper disable once MethodHasAsyncOverload
        client.Notify(noticeMessage, new Dictionary<string, object> { {"tags", tags } });
        await noticeReceived.Task;
    }
    
    [Fact]
    public async Task SendsNotice_WithFingerprint()
    {
        const string noticeMessage = "notice with tags";
        var fingerprint = Guid.NewGuid().ToString();
        var noticeReceived = new TaskCompletionSource();
        var mockHttpHandler = new Mock<HttpMessageHandler>();
        mockHttpHandler.Protected()
            .Setup<Task<HttpResponseMessage>>("SendAsync", ItExpr.IsAny<HttpRequestMessage>(), ItExpr.IsAny<CancellationToken>())
            .Callback((HttpRequestMessage request, CancellationToken token) =>
            {
                var content = request.Content?.ReadAsStringAsync(token).Result;
                var notice = JsonSerializer.Deserialize<Notice>(content!)!;
                Assert.NotNull(notice);
                Assert.Equal(noticeMessage, notice.Error.Message);
                Assert.NotNull(notice.Request);
                Assert.Equal(fingerprint, notice.Error.Fingerprint);
                noticeReceived.TrySetResult();
            })
            .ReturnsAsync(new HttpResponseMessage {
                StatusCode = HttpStatusCode.OK,
                Content = new StringContent("")
            });
        
        var options = Options.Create(new HoneybadgerOptions
        {
            ApiKey = "test",
            HttpClient = new HttpClient(mockHttpHandler.Object) { BaseAddress = new Uri("https://notavaliddomain.com") }
        });
        var client = new HoneybadgerClient(options);
        // ReSharper disable once MethodHasAsyncOverload
        client.Notify(noticeMessage, new Dictionary<string, object> { {"fingerprint", fingerprint } });
        await noticeReceived.Task;
    }

    [Fact]
    public async Task SendsNotice_WithBreadcrumbsPreservedAcrossAwait()
    {
        const string noticeMessage = "error after async work";
        const string breadcrumbBefore = "Before await";
        const string breadcrumbAfter = "After await";
        var noticeReceived = new TaskCompletionSource<Notice>();
        var mockHttpHandler = new Mock<HttpMessageHandler>();
        mockHttpHandler.Protected()
            .Setup<Task<HttpResponseMessage>>("SendAsync", ItExpr.IsAny<HttpRequestMessage>(), ItExpr.IsAny<CancellationToken>())
            .Callback((HttpRequestMessage request, CancellationToken token) =>
            {
                var content = request.Content?.ReadAsStringAsync(token).Result;
                var notice = JsonSerializer.Deserialize<Notice>(content!)!;
                noticeReceived.TrySetResult(notice);
            })
            .ReturnsAsync(new HttpResponseMessage
            {
                StatusCode = HttpStatusCode.OK,
                Content = new StringContent("")
            });

        var options = Options.Create(new HoneybadgerOptions
        {
            ApiKey = "test",
            HttpClient = new HttpClient(mockHttpHandler.Object) { BaseAddress = new Uri("https://notavaliddomain.com") }
        });
        var client = new HoneybadgerClient(options);

        client.AddBreadcrumb(breadcrumbBefore, "test");
        await Task.Yield();
        client.AddBreadcrumb(breadcrumbAfter, "test");
        await client.NotifyAsync(noticeMessage);

        var notice = await noticeReceived.Task;
        Assert.NotNull(notice.Breadcrumbs);
        Assert.True(notice.Breadcrumbs.Enabled);
        Assert.Equal(2, notice.Breadcrumbs.Trail.Length);
        Assert.Equal(breadcrumbBefore, notice.Breadcrumbs.Trail[0].Message);
        Assert.Equal(breadcrumbAfter, notice.Breadcrumbs.Trail[1].Message);
    }

}