using System.Diagnostics;
using Honeybadger.Schema;

namespace Honeybadger.NoticeHelpers;

public static class NoticeFactory
{
    /// <summary>
    /// Make a Notice from a plain message.
    /// Note: StackTrace skips 2 frames: NoticeFactory.Make + HoneybadgerClient.Notify.
    /// </summary>
    /// <param name="client"></param>
    /// <param name="message"></param>
    /// <param name="context"></param>
    /// <returns></returns>
    public static Notice Make(IHoneybadgerClient client, string message, Dictionary<string, object>? context = null)
    {
        var stackTrace = new StackTrace(2, true);

        return Make(client, message, stackTrace, context: context);
    }

    /// <summary>
    /// Make a Notice from an Exception.
    /// </summary>
    /// <param name="client"></param>
    /// <param name="error"></param>
    /// <param name="context"></param>
    /// <returns></returns>
    public static Notice Make(IHoneybadgerClient client, Exception error, Dictionary<string, object>? context = null)
    {
        // todo: Should we skip 2 frames here?
        var stackTrace = error.StackTrace == null ? new StackTrace(true) : new StackTrace(error, true);

        return Make(client, error.Message, stackTrace, error.GetType().FullName, context);
    }

    private static Notice Make(IHoneybadgerClient client, string message, StackTrace stackTrace,
        string? className = null, Dictionary<string, object>? context = null)
    {
        return new Notice(
            BreadcrumbsFactory.Get(client),
            ErrorFactory.Get(stackTrace, message, className),
            NotifierFactory.Get(),
            RequestFactory.Get(context),
            ServerFactory.Get(client)
        )
        {
            Details = null, // todo
        };
    }
}