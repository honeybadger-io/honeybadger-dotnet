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
        await _httpClient.PostAsJsonAsync(_options.ApiEndpoint, notice);
    }
}