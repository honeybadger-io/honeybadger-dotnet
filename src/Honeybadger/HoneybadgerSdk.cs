namespace Honeybadger;

public static class HoneybadgerSdk
{
    private static IHoneybadgerClient? _client = null;

    public static IHoneybadgerClient Init(HoneybadgerOptions options)
    {
        if (_client is HoneybadgerClient)
        {
            return _client;
        }

        _client = new HoneybadgerClient(options, new HttpClient());
        
        return _client;
    }
}