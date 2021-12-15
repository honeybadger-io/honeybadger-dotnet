using System.Text;
using System.Text.Json;
using Honeybadger.Schema;

namespace Honeybadger;

public class HoneybadgerClient: IHoneybadgerClient
{
    private readonly HoneybadgerOptions _options;

    private readonly HttpClient _httpClient;

    public HoneybadgerClient(HoneybadgerOptions options)
    {
        _options = options;
        _httpClient = new HttpClient();
        SetupHttpClient();
    }

    public void Notify(Notice notice)
    {
        Send(notice);
    }

    public void Notify(string message)
    {
        var notice = NoticeFactory.Make(message);
        Send(notice);
    }

    public void Notify(Exception error)
    {
        var notice = NoticeFactory.Make(error);
        Send(notice);
    }

    private async void Send(Notice notice)
    {
        // todo: change to route to dotnet
        var request = new HttpRequestMessage(HttpMethod.Post, "v1/notices/js");
        request.Content = new StringContent(JsonSerializer.Serialize(notice), Encoding.UTF8, "application/json");
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
        _httpClient.BaseAddress = _options.Endpoint;
        _httpClient.DefaultRequestHeaders.Add("X-API-Key", _options.ApiKey);
    }
}