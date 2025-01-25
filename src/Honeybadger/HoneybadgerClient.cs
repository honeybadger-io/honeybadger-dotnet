using System.Text;
using System.Text.Json;
using System.Text.Json.Serialization;
using Honeybadger.NoticeHelpers;
using Honeybadger.Schema;
using Microsoft.Extensions.Options;

namespace Honeybadger;

public class HoneybadgerClient : IHoneybadgerClient, IDisposable
{
    public HoneybadgerOptions Options { get; private set; } = null!;
    
    private HttpClient? _httpClient;

    private readonly ThreadLocal<Dictionary<string, object>> _context;

    private readonly ThreadLocal<List<Trail>> _breadcrumbs;

    public HoneybadgerClient(IOptions<HoneybadgerOptions> options)
    {
        _context = new ThreadLocal<Dictionary<string, object>>(() => new Dictionary<string, object>());
        _breadcrumbs = new ThreadLocal<List<Trail>>(() => new List<Trail>());
        Configure(options.Value);
    }

    public void Notify(string message)
    {
        Notify(message, new Dictionary<string, object>());
    }

    public void Notify(string message, Dictionary<string, object> context)
    {
        var notice = NoticeFactory.Make(this, message, GetContext(context));
        Notify(notice);
    }

    public void Notify(Exception error)
    {
        Notify(error, new Dictionary<string, object>());
    }

    public void Notify(Exception error, Dictionary<string, object> context)
    {
        var notice = NoticeFactory.Make(this, error, GetContext(context));
        Notify(notice);
    }

    private void Notify(Notice notice)
    {
        Send(notice).Wait();
    }

    public Task NotifyAsync(string message)
    {
        return NotifyAsync(message, new Dictionary<string, object>());
    }

    public Task NotifyAsync(string message, Dictionary<string, object> context)
    {
        var notice = NoticeFactory.Make(this, message, GetContext(context));
        return NotifyAsync(notice);
    }

    public Task NotifyAsync(Exception error)
    {
        return NotifyAsync(error, new Dictionary<string, object>());
    }

    public Task NotifyAsync(Exception error, Dictionary<string, object> context)
    {
        var notice = NoticeFactory.Make(this, error, GetContext(context));
        return NotifyAsync(notice);
    }

    private Task NotifyAsync(Notice notice)
    {
        return Send(notice);
    }

    public void AddContext(Dictionary<string, object> context)
    {
        if (_context.Value == null)
        {
            return;
        }
        
        foreach (var entry in context)
        {
            _context.Value[entry.Key] = entry.Value;
        }
    }

    public void ResetContext()
    {
        _context.Value?.Clear();
    }

    public void AddBreadcrumb(string message, string? category = null, Dictionary<string, object?>? options = null)
    {
        if (_breadcrumbs.Value == null || !Options.ShouldReport() || !Options.BreadcrumbsEnabled)
        {
            // fixme: for debugging purposes (see #3 - CI is randomly failing) 
            // Console.WriteLine($"not adding breadcrumb: {message}");
            return;
        }

        var trail = new Trail(message);
        if (category != null)
        {
            trail.Category = category;
        }

        if (options != null)
        {
            trail.Metadata = options;
        }
        
        _breadcrumbs.Value.Add(trail);

        if (_breadcrumbs.Value.Count > Options.MaxBreadcrumbs)
        {
            _breadcrumbs.Value.RemoveRange(0, _breadcrumbs.Value.Count - Options.MaxBreadcrumbs);
        }
    }

    public void ResetBreadcrumbs()
    {
        _breadcrumbs.Value?.Clear();
    }

    public void Configure(HoneybadgerOptions options)
    {
        Options = options;
        GetClient(true);
    }
    
    public void Dispose()
    {
        _context.Dispose();
        _breadcrumbs.Dispose();
    }

    private Dictionary<string, object> GetContext(Dictionary<string, object>? context = null)
    {
        if (context == null)
        {
            return _context.Value ?? new Dictionary<string, object>();
        }

        if (_context.Value == null)
        {
            return context;
        }

        foreach (var entry in context)
        {
            _context.Value[entry.Key] = entry.Value;
        }

        return _context.Value;
    }

    Trail[]? IHoneybadgerClient.GetBreadcrumbs()
    {
        return Options.BreadcrumbsEnabled ? _breadcrumbs.Value?.ToArray() : null;
    }

    private async Task Send(Notice notice)
    {
        // Console.WriteLine("Ready to send report to Honeybadger");
        var request = new HttpRequestMessage(HttpMethod.Post, "v1/notices");
        var json = JsonSerializer.Serialize(notice, new JsonSerializerOptions
        {
            DefaultIgnoreCondition = JsonIgnoreCondition.WhenWritingNull
        });
        request.Content = new StringContent(json, Encoding.UTF8, "application/json");
        try
        {
            var result = await GetClient().SendAsync(request);
            if (!result.IsSuccessStatusCode)
            {
                // var content = await result.Content.ReadAsStringAsync();
                // Console.WriteLine("Could not send report to Honeybadger | HTTP[{0}]: {1}", result.StatusCode, content);
            }
            else
            {
                // Console.WriteLine("Report sent to Honeybadger");
            }
        }
        catch (Exception ex)
        {
            await Console.Error.WriteLineAsync(ex.Message);
        }
    }

    private HttpClient GetClient(bool forceNew = false)
    {
        if (_httpClient is not null && !forceNew)
        {
            return _httpClient;
        }
        
        if (Options.HttpClient is not null)
        {
            _httpClient = Options.HttpClient;
        }
        else
        {
            var client = new HttpClient();
            client.BaseAddress = Options.Endpoint;
            client.DefaultRequestHeaders.Add("X-API-Key", Options.ApiKey);
            _httpClient = client;
        }
        
        return _httpClient;
    }
}