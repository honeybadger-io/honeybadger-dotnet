using System.Diagnostics;
using System.Reflection;
using Honeybadger.Schema;

namespace Honeybadger;

public static class NoticeFactory
{
    public static Notice Make(string message)
    {
        var stackTrace = new StackTrace(true);
        
        return Make(message, stackTrace);
    }

    public static Notice Make(Exception error)
    {
        var stackTrace = new StackTrace(error);
        
        return Make(error.Message, stackTrace);
    }

    private static Notice Make(string message, StackTrace stackTrace)
    {
        var firstFrame = stackTrace.GetFrame(0);
        var notice = new Notice
        {
            Notifier = GetNotifier(),
            Breadcrumbs = GetBreadcrumbs(),
            Details = null, // todo
            Error = new Error
            {
                Class = firstFrame?.GetType().FullName,
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