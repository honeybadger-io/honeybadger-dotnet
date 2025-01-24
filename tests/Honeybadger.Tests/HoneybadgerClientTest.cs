using System.Collections.Generic;
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
        var options = Options.Create(new HoneybadgerOptions("test")
        {
            ReportData = false
        });
        
        var client = new HoneybadgerClient(options);
        Assert.NotNull(client);
    }
    
    [Fact]
    public void SendsNotice_WithBreadcrumbs()
    {
        const string noticeMessage = "notice with breadcrumbs";
        const string breadcrumbMessage = "breadcrumb";
        const string breadcrumbCategory = "test";
        var breadcrumbMetadata = new Dictionary<string, object?>()
        {
            {"metadata", "value"}
        };
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
                Assert.Equal(breadcrumbMetadata, firstTrail.Metadata);
                
                
            })
            .ReturnsAsync(new HttpResponseMessage {
                StatusCode = HttpStatusCode.OK,
                Content = new StringContent("")
            });
        
        var options = Options.Create(new HoneybadgerOptions("test")
        {
            HttpClient = new HttpClient(mockHttpHandler.Object)
        });
        var client = new HoneybadgerClient(options);
        client.AddBreadcrumb(breadcrumbMessage, breadcrumbCategory, breadcrumbMetadata);
        client.Notify(noticeMessage);
    }
    
    [Fact]
    public void SendsNotice_WithContext()
    {
        const string noticeMessage = "notice with context";
        var noticeContext = new Dictionary<string, object>()
        {
            {"user_id", 123}
        };
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
                Assert.Equal(noticeContext, notice.Request.Context);
            })
            .ReturnsAsync(new HttpResponseMessage {
                StatusCode = HttpStatusCode.OK,
                Content = new StringContent("")
            });
        
        var options = Options.Create(new HoneybadgerOptions("test")
        {
            HttpClient = new HttpClient(mockHttpHandler.Object)
        });
        var client = new HoneybadgerClient(options);
        client.AddContext(noticeContext);
        client.Notify(noticeMessage);
    }

}