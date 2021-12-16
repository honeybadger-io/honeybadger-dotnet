using System.Diagnostics;
using System.Globalization;
using System.Reflection;
using Honeybadger.Schema;

namespace Honeybadger;

public static class NoticeFactory
{
    public static Notice Make(IHoneybadgerClient client, string message)
    {
        // skip frame from NoticeFactory.Make + HoneybadgerClient.Notify
        var stackTrace = new StackTrace(2,true);
        
        return Make(client, message, stackTrace);
    }

    public static Notice Make(IHoneybadgerClient client, Exception error)
    {
        var stackTrace = error.StackTrace == null ? new StackTrace(true) : new StackTrace(error, true);
        
        return Make(client, error.Message, stackTrace, error.GetType().FullName);
    }

    private static Notice Make(IHoneybadgerClient client, string message, StackTrace stackTrace, string? className = null)
    {
        var notice = new Notice
        {
            Notifier = GetNotifier(),
            Breadcrumbs = GetBreadcrumbs(),
            Details = null, // todo
            Request = null, // todo
            Server = GetServer(client),
            Error = new Error
            {
                Class = className ?? stackTrace.GetFrame(0)?.GetMethod()?.DeclaringType?.FullName,
                Message = message,
                Backtrace = GetBacktrace(stackTrace),
                Fingerprint = null, // todo
                Tags = null, // todo
            },
            
        };

        return notice;
    }

    /**
     * TODO: move to a separate file
     */
    private static Server GetServer(IHoneybadgerClient client)
    {
        return new Server
        {
            Hostname = client.Options?.HostName,
            Revision = client.Options?.Revision,
            EnvironmentName = client.Options?.AppEnvironment,
            ProjectRoot = client.Options?.ProjectRoot,
            Time = DateTime.UtcNow.ToString(CultureInfo.InvariantCulture),
            Pid = Environment.ProcessId,
            Stats = null // todo
        };
    }

    /**
     * TODO: move to a separate file
     */
    private static ErrorBacktrace[] GetBacktrace(StackTrace stackTrace)
    {
        var result = new List<ErrorBacktrace>();
        for (var i = 0; i < stackTrace.FrameCount; i++)
        {
            var sf = stackTrace.GetFrame(i);
            if (sf == null)
            {
                continue;
            }

            result.Add(new ErrorBacktrace
            {
                Class = sf.GetMethod()?.DeclaringType?.FullName,
                Type = null, // todo
                Column = sf.GetFileColumnNumber().ToString(),
                File = sf.GetFileName(),
                Number = sf.GetFileLineNumber().ToString(),
                Method = sf.GetMethod()?.Name,
                Context = null, // todo
                Source = null, // todo
                Args = null, // todo
            });
        }

        return result.ToArray();
    }

    /**
     * TODO: move to a separate file
     */
    private static Notifier GetNotifier()
    {
        return new Notifier
        {
            Name = Assembly.GetCallingAssembly().GetName().Name,
            Url = "https://github.com/honeybadger-io/honeybadger-dotnet",
            Version = Assembly.GetCallingAssembly().GetName().Version?.ToString()
        };
    }

    /**
     * TODO: move to a separate file
     */
    private static Breadcrumbs GetBreadcrumbs()
    {
        return new Breadcrumbs
        {
            Enabled = false
        };
    }
}