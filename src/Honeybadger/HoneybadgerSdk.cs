namespace Honeybadger;

public static class HoneybadgerSdk
{
    private static IHoneybadgerClient _client = new NullClient();

    public static IHoneybadgerClient Init(HoneybadgerOptions options)
    {
        if (!options.ReportData)
        {
            return _client;
        }

        _client = new HoneybadgerClient(options);
        return _client;
    }
}