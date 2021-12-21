using System.Formats.Asn1;
using System.Text;
using System.Text.Json;
using System.Text.Json.Serialization;
using Honeybadger.NoticeHelpers;
using Honeybadger.Schema;

namespace Honeybadger;

public class HoneybadgerClient : IHoneybadgerClient
{
    public HoneybadgerOptions Options { get; }

    private readonly HttpClient _httpClient;

    private readonly ThreadLocal<Dictionary<string, object>> _context;

    private readonly ThreadLocal<List<Trail>> _breadcrumbs;

    public HoneybadgerClient(HoneybadgerOptions options, HttpClient httpClient)
    {
        _httpClient = httpClient;
        _context = new ThreadLocal<Dictionary<string, object>>(() => new Dictionary<string, object>());
        _breadcrumbs = new ThreadLocal<List<Trail>>(() => new List<Trail>());
        Options = options;
        SetupHttpClient();
    }

    public void Notify(string message)
    {
        Notify(message, new Dictionary<string, object>());
    }

    public void Notify(string message, Dictionary<string, object> context)
    {
        var notice = NoticeFactory.Make(this, message, GetContext(context));
        Send(notice);
    }

    public void Notify(Exception error)
    {
        Notify(error, new Dictionary<string, object>());
    }

    public void Notify(Exception error, Dictionary<string, object> context)
    {
        var notice = NoticeFactory.Make(this, error, GetContext(context));
        Send(notice);
    }

    public void AddContext(Dictionary<string, object> context)
    {
        if (_context.Value == null)
        {
            return;
        }
        
        foreach (var (key, value) in context)
        {
            _context.Value[key] = value;
        }
    }

    public void ResetContext()
    {
        _context.Value?.Clear();
    }

    public void AddBreadcrumb(string message, string? category = null, Dictionary<string, object?>? options = null)
    {
        if (_breadcrumbs.Value == null || !Options.ReportData || !Options.BreadcrumbsEnabled)
        {
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

        foreach (var (key, value) in context)
        {
            _context.Value[key] = value;
        }

        return _context.Value;
    }

    Trail[]? IHoneybadgerClient.GetBreadcrumbs()
    {
        return Options.BreadcrumbsEnabled ? _breadcrumbs.Value?.ToArray() : null;
    }

    private async void Send(Notice notice)
    {
        Console.WriteLine("Ready to send report to Honeybadger");
        var request = new HttpRequestMessage(HttpMethod.Post, "v1/notices");
        var json = JsonSerializer.Serialize(notice, new JsonSerializerOptions
        {
            DefaultIgnoreCondition = JsonIgnoreCondition.WhenWritingNull
        });
        request.Content = new StringContent(json, Encoding.UTF8, "application/json");
        try
        {
            var result = await _httpClient.SendAsync(request);
            if (!result.IsSuccessStatusCode)
            {
                var content = await result.Content.ReadAsStringAsync();
                Console.WriteLine("Could not send report to Honeybadger | HTTP[{0}]: {1}", result.StatusCode, content);
            }
            else
            {
                Console.WriteLine("Report sent to Honeybadger");
            }
        }
        catch (Exception ex)
        {
            Console.Error.WriteLine(ex.Message);
        }
    }

    private void SetupHttpClient()
    {
        _httpClient.BaseAddress = Options.Endpoint;
        _httpClient.DefaultRequestHeaders.Add("X-API-Key", Options.ApiKey);
    }
}