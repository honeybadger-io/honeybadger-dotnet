using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Text.Json;
using System.Threading;
using System.Threading.Tasks;
using Moq;
using Moq.Protected;

namespace Honeybadger.DotNetCore.Tests.Fixture;

public class HttpClientMock
{
    private readonly HttpClient _httpClient;
    private readonly List<CapturedRequest> _capturedRequests = new();

    public HttpClientMock()
    {
        var mockHandler = new Mock<HttpMessageHandler>();
        
        // Setup the handler to capture requests and return a successful response
        mockHandler.Protected()
            .Setup<Task<HttpResponseMessage>>("SendAsync", 
                ItExpr.IsAny<HttpRequestMessage>(), 
                ItExpr.IsAny<CancellationToken>())
            .Returns<HttpRequestMessage, CancellationToken>(async (request, cancellationToken) =>
            {
                // Capture the request
                var body = request.Content != null ? await request.Content.ReadAsStringAsync(cancellationToken) : null;
                var capturedRequest = new CapturedRequest
                {
                    Method = request.Method,
                    RequestUri = request.RequestUri,
                    Headers = request.Headers.ToDictionary(h => h.Key, h => string.Join(", ", h.Value)),
                    Body = body
                };
                
                _capturedRequests.Add(capturedRequest);
                
                // Return a successful response
                return new HttpResponseMessage(System.Net.HttpStatusCode.Created);
            });

        _httpClient = new HttpClient(mockHandler.Object);
        _httpClient.BaseAddress = new Uri("https://api.honeybadger.io/");
    }

    public HttpClient Client => _httpClient;

    public List<CapturedRequest> CapturedRequests => _capturedRequests;

    public CapturedRequest? LastRequest => _capturedRequests.LastOrDefault();

    public T? GetRequestBodyAs<T>() where T : class
    {
        var lastRequest = LastRequest;
        if (lastRequest?.Body == null) return null;
        
        return JsonSerializer.Deserialize<T>(lastRequest.Body, new JsonSerializerOptions
        {
            PropertyNameCaseInsensitive = true
        });
    }

    public void ClearCapturedRequests()
    {
        _capturedRequests.Clear();
    }
}

public class CapturedRequest
{
    public HttpMethod Method { get; set; } = null!;
    public Uri? RequestUri { get; set; }
    public Dictionary<string, string> Headers { get; set; } = new();
    public string? Body { get; set; }
}