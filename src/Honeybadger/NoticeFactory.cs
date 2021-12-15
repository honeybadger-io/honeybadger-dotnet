using System.Diagnostics;
using System.Reflection;
using Honeybadger.Schema;

namespace Honeybadger;

public static class NoticeFactory
{
    public static Notice Make(string message)
    {
        // skip frame from NoticeFactory
        var stackTrace = new StackTrace(1,true);
        
        return Make(message, stackTrace);
    }

    public static Notice Make(Exception error)
    {
        var stackTrace = error.StackTrace == null ? new StackTrace(true) : new StackTrace(error, true);
        
        return Make(error.Message, stackTrace, error.GetType().FullName);
    }

    private static Notice Make(string message, StackTrace stackTrace, string? className = null)
    {
        var notice = new Notice
        {
            Notifier = GetNotifier(),
            Breadcrumbs = GetBreadcrumbs(),
            Details = null, // todo
            Error = new Error
            {
                Class = className ?? stackTrace.GetFrame(0)?.GetMethod()?.DeclaringType?.FullName,
                Message = message,
                Backtrace = GetBacktrace(stackTrace),
                Fingerprint = null, // todo
                Source = null, // todo: error.Source ?
                Causes = null, // todo
                Tags = null, // todo
            },
            Request = null, // todo
            Server = null // todo
        };

        return notice;
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
                Class = null, // todo
                Type = null, // todo
                Column = sf.GetFileColumnNumber(),
                File = sf.GetFileName(),
                Number = sf.GetFileColumnNumber(),
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
            Enabled = false,
            Trail = null
        };
    }
}