using Honeybadger.Schema;

namespace Honeybadger.NoticeHelpers;

public static class ServerFactory
{
    public static Server Get(IHoneybadgerClient client)
    {
        return new Server
        {
            Hostname = client.Options?.HostName,
            Revision = client.Options?.Revision,
            EnvironmentName = client.Options?.AppEnvironment,
            ProjectRoot = client.Options?.ProjectRoot,
            Stats = null // todo
        };
    }
}