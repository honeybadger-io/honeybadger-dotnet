namespace Honeybadger;

public static class HoneybadgerSdk
{
    private static IHoneybadgerClient _client = new NullClient();

    public static IHoneybadgerClient Init(HoneybadgerOptions options)
    {
        // 1. validate options (that should come as parameters - or from config file)
        // 2. if options are invalid -> throw
        // 3. if options mark honeybadger as disabled return NullClient
        // 4. otherwise return new HoneybadgerClient
        if (options.IsDisabled)
        {
            return _client;
        }

        _client = new HoneybadgerClient(options);
        return _client;
    }
}