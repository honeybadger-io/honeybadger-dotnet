using System;
using System.Collections.Generic;
using System.IO;
using System.Net.Http;
using System.Text.Json;
using System.Threading.Tasks;
using Honeybadger.DotNetCore.Tests.Fixture;
using Honeybadger.Schema;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.TestHost;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Xunit;

namespace Honeybadger.DotNetCore.Tests;

public class RequestDataTests
{
    [Fact]
    public async Task UnnamedMinimalApiRoute_CapturesAction()
    {
        // Arrange
        var mockHbClient = new HttpClientMock();
        var server = await StartTestServer(mockHbClient);
        var client = server.GetTestClient();
        
        // Act
        try
        {
            await client.GetAsync("/debug");
        }
        catch (Exception ex)
        {
            // test server propagates exceptions to the unit test
            Assert.Equal("Hello from .Net Core Web App!", ex.Message);
        }

        // Assert
        var notice = mockHbClient.GetRequestBodyAs<Notice>();
        Assert.NotNull(notice);
        Assert.NotNull(notice.Request);
        
        Assert.Equal("GET", notice.Request.CgiData?.RequestMethod);
        Assert.Equal("HTTP: GET /debug", notice.Request.Action);
        Assert.Equal("http://localhost/debug", notice.Request.Url);
    }

    [Fact]
    public async Task NamedMinimalApiRoute_CapturesAction()
    {
        // Arrange
        var mockHbClient = new HttpClientMock();
        var server = await StartTestServer(mockHbClient);
        var client = server.GetTestClient();
        
        // Act
        try
        {
            await client.GetAsync("/named-debug");
        }
        catch (Exception ex)
        {
            // test server propagates exceptions to the unit test
            Assert.Equal("Hello from .Net Core Web App!", ex.Message);
        }

        // Assert
        var notice = mockHbClient.GetRequestBodyAs<Notice>();
        Assert.NotNull(notice);
        Assert.NotNull(notice.Request);
        
        Assert.Equal("GET", notice.Request.CgiData?.RequestMethod);
        Assert.Equal("Debug", notice.Request.Action);
        Assert.Equal("http://localhost/named-debug", notice.Request.Url);
    }

    [Fact]
    public async Task ControllerRoute_CapturesActionAndComponent()
    {
        // Arrange
        var mockHbClient = new HttpClientMock();
        var server = await StartTestServer(mockHbClient);
        var client = server.GetTestClient();
        
        // Act
        try
        {
            var resp = await client.GetAsync("/Test/Debug");
        }
        catch (Exception ex)
        {
            // test server propagates exceptions to the unit test
            Assert.Equal("Hello from .Net Core Web App!", ex.Message);
        }

        // Assert
        var notice = mockHbClient.GetRequestBodyAs<Notice>();
        Assert.NotNull(notice);
        Assert.NotNull(notice.Request);
        
        Assert.Equal("GET", notice.Request.CgiData?.RequestMethod);
        Assert.Equal("Test", notice.Request.Component);
        // We did not name this endpoint, so we get the default DisplayName
        Assert.Equal("Honeybadger.DotNetCore.Tests.Fixture.TestController.Debug (Honeybadger.DotNetCore.Tests)", notice.Request.Action);
        Assert.Equal("http://localhost/Test/Debug", notice.Request.Url);
    }

    [Fact]
    public async Task RequestWithQueryParameters_CapturesQueryData()
    {
        // Arrange
        var mockHbClient = new HttpClientMock();
        var server = await StartTestServer(mockHbClient);
        var client = server.GetTestClient();
        
        // Act
        try
        {
            await client.GetAsync("/debug?param1=value1&param2=value2");
        }
        catch (Exception ex)
        {
            // test server propagates exceptions to the unit test
            Assert.Equal("Hello from .Net Core Web App!", ex.Message);
        }
        
        // Assert
        var notice = mockHbClient.GetRequestBodyAs<Notice>();
        Assert.NotNull(notice?.Request);
        Assert.Contains("param1=value1", notice.Request.Url);
        Assert.Contains("param2=value2", notice.Request.Url);
    }

    [Fact]
    public async Task RequestWithHeaders_CapturesHeaders()
    {
        // Arrange
        var mockHbClient = new HttpClientMock();
        var server = await StartTestServer(mockHbClient);
        var client = server.GetTestClient();
        client.DefaultRequestHeaders.Add("User-Agent", "TestAgent/1.0");
        client.DefaultRequestHeaders.Add("X-Custom-Header", "CustomValue");
        
        // Act
        try
        {
            await client.GetAsync("/debug");
        }
        catch (Exception ex)
        {
            // test server propagates exceptions to the unit test
            Assert.Equal("Hello from .Net Core Web App!", ex.Message);
        }
        
        // Assert
        var notice = mockHbClient.GetRequestBodyAs<Notice>();
        Assert.NotNull(notice?.Request.CgiData);
        Assert.Contains("TestAgent/1.0", notice.Request.CgiData.HttpUserAgent ?? "");
    }

    [Fact]
    public async Task RequestWithCookies_CapturesCookies()
    {
        // Arrange
        var mockHbClient = new HttpClientMock();
        var server = await StartTestServer(mockHbClient);
        var client = server.GetTestClient();
        client.DefaultRequestHeaders.Add("Cookie", "sessionId=abc123; userId=456");
        
        // Act
        try
        {
            await client.GetAsync("/debug");
        }
        catch (Exception ex)
        {
            // test server propagates exceptions to the unit test
            Assert.Equal("Hello from .Net Core Web App!", ex.Message);
        }
        
        // Assert
        var notice = mockHbClient.GetRequestBodyAs<Notice>();
        Assert.NotNull(notice?.Request.CgiData);
        Assert.Contains("sessionId=abc123", notice.Request.CgiData.HttpCookie ?? "");
    }

    [Fact]
    public async Task RequestWithReferer_CapturesReferer()
    {
        // Arrange
        var mockHbClient = new HttpClientMock();
        var server = await StartTestServer(mockHbClient);
        var client = server.GetTestClient();
        client.DefaultRequestHeaders.Add("Referer", "https://example.com/previous-page");
        
        // Act
        try
        {
            await client.GetAsync("/debug");
        }
        catch (Exception ex)
        {
            // test server propagates exceptions to the unit test
            Assert.Equal("Hello from .Net Core Web App!", ex.Message);
        }
        
        // Assert
        var notice = mockHbClient.GetRequestBodyAs<Notice>();
        Assert.NotNull(notice?.Request.CgiData);
        Assert.Equal("https://example.com/previous-page", notice.Request.CgiData.HttpReferer);
    }

    [Fact]
    public async Task RequestWithForwardedFor_CapturesForwardedFor()
    {
        // Arrange
        var mockHbClient = new HttpClientMock();
        var server = await StartTestServer(mockHbClient);
        var client = server.GetTestClient();
        client.DefaultRequestHeaders.Add("X-Forwarded-For", "192.168.1.1, 10.0.0.1");
        
        // Act
        try
        {
            await client.GetAsync("/debug");
        }
        catch (Exception ex)
        {
            // test server propagates exceptions to the unit test
            Assert.Equal("Hello from .Net Core Web App!", ex.Message);
        }
        
        // Assert
        var notice = mockHbClient.GetRequestBodyAs<Notice>();
        Assert.NotNull(notice?.Request.CgiData);
        Assert.Equal("192.168.1.1, 10.0.0.1", notice.Request.CgiData.HttpXForwardedFor);
    }

    [Fact]
    public async Task RequestCapturesHostHeader()
    {
        // Arrange
        var mockHbClient = new HttpClientMock();
        var server = await StartTestServer(mockHbClient);
        var client = server.GetTestClient();
        
        // Act
        try
        {
            await client.GetAsync("/debug");
        }
        catch (Exception ex)
        {
            // test server propagates exceptions to the unit test
            Assert.Equal("Hello from .Net Core Web App!", ex.Message);
        }
        
        // Assert
        var notice = mockHbClient.GetRequestBodyAs<Notice>();
        Assert.NotNull(notice?.Request.CgiData);
        Assert.NotNull(notice.Request.CgiData.HttpHost);
        Assert.Contains("localhost", notice.Request.CgiData.HttpHost);
    }

    [Fact(Skip = "Not implemented yet")]
    public async Task PostRequestWithBody_CapturesRequestBody()
    {
        // Arrange
        var mockHbClient = new HttpClientMock();
        var server = await StartTestServer(mockHbClient);
        var client = server.GetTestClient();
        var requestBody = new
        {
            message = "test request body data",
            userId = 123,
        };
        
        // Act
        try
        {
            var rawRequestBody = JsonSerializer.Serialize(requestBody);
            var content = new StringContent(
                rawRequestBody, 
                System.Text.Encoding.UTF8,
                "application/json");
            await client.PostAsync("/debug", content);
        }
        catch (Exception ex)
        {
            // test server propagates exceptions to the unit test
            Assert.Equal("Hello from .Net Core Web App!", ex.Message);
        }
        
        // Assert
        var notice = mockHbClient.GetRequestBodyAs<Notice>();
        Assert.NotNull(notice?.Request);
        Assert.Equal("POST", notice.Request.CgiData?.RequestMethod);
        Assert.Equal("http://localhost/debug", notice.Request.Url);
        
        // Request body should be captured in the Params property
        Assert.NotNull(notice.Request.Params);
        Assert.True(notice.Request.Params.ContainsKey("message"));
        Assert.Equal("test request body data", notice.Request.Params["message"].ToString());
        Assert.True(notice.Request.Params.ContainsKey("userId"));
        Assert.Equal("123", notice.Request.Params["userId"].ToString());
    }
    
    [Fact(Skip = "Not implemented yet")]
    public async Task PostRequestWithBody_CapturesRequestBodyWithFilteredKeys()
    {
        // Arrange
        var mockHbClient = new HttpClientMock();
        var server = await StartTestServer(mockHbClient);
        var client = server.GetTestClient();
        var requestBody = new
        {
            username = "user123",
            password = "pass123",
        };
        
        // Act
        try
        {
            var rawRequestBody = JsonSerializer.Serialize(requestBody);
            var content = new StringContent(
                rawRequestBody, 
                System.Text.Encoding.UTF8,
                "application/json");
            await client.PostAsync("/debug", content);
        }
        catch (Exception ex)
        {
            // test server propagates exceptions to the unit test
            Assert.Equal("Hello from .Net Core Web App!", ex.Message);
        }
        
        // Assert
        var notice = mockHbClient.GetRequestBodyAs<Notice>();
        Assert.NotNull(notice?.Request);
        Assert.Equal("POST", notice.Request.CgiData?.RequestMethod);
        Assert.Equal("http://localhost/debug", notice.Request.Url);
        
        // Request body should be captured in the Params property
        Assert.NotNull(notice.Request.Params);
        Assert.True(notice.Request.Params.ContainsKey("username"));
        Assert.Equal("user123", notice.Request.Params["username"].ToString());
        Assert.True(notice.Request.Params.ContainsKey("password"));
        Assert.Equal("[FILTERED]", notice.Request.Params["password"].ToString());
    }
    
    [Fact(Skip = "Not implemented yet")]
    public async Task PostRequestWithMultiPartForm_CapturesRequestBody()
    {
        // Arrange
        var mockHbClient = new HttpClientMock();
        var server = await StartTestServer(mockHbClient);
        var client = server.GetTestClient();
        var form = new MultipartFormDataContent();
        form.Add(new StringContent("user123"), "username");
        form.Add(new StringContent("user123@email.com"), "email");
        form.Add(new StringContent("pass123"), "password");            
        form.Add(new ByteArrayContent([0,0,0,0], 0, 4), "dummy");
        
        // Act
        try
        {
            await client.PostAsync("/debug-form", form);
        }
        catch (Exception ex)
        {
            // test server propagates exceptions to the unit test
            Assert.Equal("Hello from .Net Core Web App!", ex.Message);
        }
        
        // Assert
        var notice = mockHbClient.GetRequestBodyAs<Notice>();
        Assert.NotNull(notice?.Request);
        Assert.Equal("POST", notice.Request.CgiData?.RequestMethod);
        Assert.Equal("http://localhost/debug-form", notice.Request.Url);
        
        // Request body should be captured in the Params property
        Assert.NotNull(notice.Request.Params);
        Assert.True(notice.Request.Params.ContainsKey("username"));
        Assert.Equal("user123", notice.Request.Params["username"].ToString());
        Assert.True(notice.Request.Params.ContainsKey("email"));
        Assert.Equal("user123@email.com", notice.Request.Params["email"].ToString());
        Assert.True(notice.Request.Params.ContainsKey("password"));
        Assert.Equal("[FILTERED]", notice.Request.Params["password"].ToString());
        Assert.True(notice.Request.Params.ContainsKey("dummy"));
    }
    
    /// <summary>
    /// Microsoft's documentation on how to test middleware:
    /// https://learn.microsoft.com/en-us/aspnet/core/test/middleware?view=aspnetcore-9.0
    /// </summary>
    private static async Task<IHost> StartTestServer(HttpClientMock mockHbClient)
    {
        return await new HostBuilder()
            .ConfigureWebHost(webBuilder =>
            {
                webBuilder
                    .UseTestServer()
                    .ConfigureServices(services =>
                    {
                        services.AddRouting();
                        // Configure Honeybadger with our mock HttpClient
                        services.Configure<HoneybadgerOptions>(options =>
                        {
                            options.ApiKey = "test-api-key";
                            options.ReportData = true;
                            options.HttpClient = mockHbClient.Client;
                        });
                        services.AddHoneybadger();
                        services.AddControllers();
                    })
                    .Configure(app =>
                    {
                        app.UseRouting();
                        app.UseEndpoints(endpoints =>
                        {
                            endpoints
                                .MapGet("/debug", _ => throw new Exception("Hello from .Net Core Web App!"));
                            
                            endpoints
                                .MapPost("/debug", async context =>
                                {
                                    // read the body once
                                    await context.Request.ReadFromJsonAsync<Dictionary<string, object>>();
                                    
                                    throw new Exception("Hello from .Net Core Web App!");
                                });
                            
                            endpoints
                                .MapPost("/debug-form", async context =>
                                {
                                    await context.Request.ReadFormAsync();
                                    
                                    throw new Exception("Hello from .Net Core Web App!");
                                });
                            
                            endpoints
                                .MapGet("/named-debug", _ => throw new Exception("Hello from .Net Core Web App!"))
                                .WithName("Debug");
                            
                            endpoints.MapControllers();
                        });
                    });
            })
            .StartAsync();
    }
}