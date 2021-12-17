using System.Text;
using System.Text.Json;
using System.Text.Json.Serialization;
using Honeybadger.NoticeHelpers;
using Honeybadger.Schema;

namespace Honeybadger;

public class HoneybadgerClient: IHoneybadgerClient
{
    public HoneybadgerOptions Options { get; }
    
    private readonly HttpClient _httpClient;

    private readonly ThreadLocal<Dictionary<string, object>> _context;

    public HoneybadgerClient(HoneybadgerOptions options)
    {
        _httpClient = new HttpClient();
        _context = new ThreadLocal<Dictionary<string, object>>(() => new Dictionary<string, object>());
        Options = options;
        SetupHttpClient();
    }
    
    public void Notify(string message, Dictionary<string, object>? context = null)
    {
        var notice = NoticeFactory.Make(this, message, GetContext(context));
        Send(notice);
    }

    public void Notify(Exception error, Dictionary<string, object>? context = null)
    {
        var notice = NoticeFactory.Make(this, error, GetContext(context));
        Send(notice);
    }

    public void AddContext(Dictionary<string, object> context)
    {
        foreach (var (key,value) in context)
        {
            if (_context.Value != null)
            {
                _context.Value[key] = value;
            }
        }
    }

    public void ResetContext()
    {
        _context.Value?.Clear();
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

    private async void Send(Notice notice)
    {
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