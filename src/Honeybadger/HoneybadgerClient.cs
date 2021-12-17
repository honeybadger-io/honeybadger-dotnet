using System.Text;
using System.Text.Json;
using System.Text.Json.Serialization;
using Honeybadger.Schema;

namespace Honeybadger;

public class HoneybadgerClient: IHoneybadgerClient
{
    public HoneybadgerOptions Options { get; }
    
    private readonly HttpClient _httpClient;

    public HoneybadgerClient(HoneybadgerOptions options)
    {
        _httpClient = new HttpClient();
        Options = options;
        SetupHttpClient();
    }

    public void Notify(Notice notice)
    {
        Send(notice);
    }

    public void Notify(string message)
    {
        var notice = NoticeFactory.Make(this, message);
        Send(notice);
    }

    public void Notify(Exception error)
    {
        var notice = NoticeFactory.Make(this, error);
        Send(notice);
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