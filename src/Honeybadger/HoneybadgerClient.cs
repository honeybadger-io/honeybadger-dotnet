using System.Net.Http.Json;
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
    }

    public async void Notify(Notice notice)
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
        await _httpClient.PostAsJsonAsync(_options.Endpoint, notice);
    }
}