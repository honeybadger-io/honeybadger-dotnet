using System.Diagnostics;
using Honeybadger.NoticeHelpers;
using Honeybadger.Schema;

namespace Honeybadger;

public abstract class NoticeFactory
{
    public abstract void EnrichNotice(IHoneybadgerClient client, Notice notice);
    
    /// <summary>
    /// Make a Notice from a plain message.
    /// Note: Some stack frames are skipped to avoid internal Honeybadger being included stack trace.
    /// Alternatively, you can pass a framesToSkip parameter to skip a specific number of frames.
    /// </summary>
    public Notice Make(IHoneybadgerClient client, 
        string message, 
        Dictionary<string, object>? hbContext = null, 
        int? framesToSkip = null)
    {
        StackTrace stackTrace;
        if (framesToSkip is not null)
        {
            stackTrace = new StackTrace(framesToSkip.Value, true);    
        }
        else
        {
            stackTrace = new StackTrace(1, true);
            // do while loop to skip internal frames
            var frameIndex = 0;
            var isFrameInternal = true;
            while (isFrameInternal)
            {
                isFrameInternal = stackTrace.GetFrame(frameIndex)?
                    .GetMethod()?
                    .DeclaringType?.FullName?
                    .Contains("Honeybadger") ?? false;
            
                frameIndex++;
            }
            stackTrace = new StackTrace(frameIndex, true);
        }
        
        return Make(client, message, stackTrace, hbContext: hbContext);
    }

    /// <summary>
    /// Make a Notice from an Exception.
    /// </summary>
    public Notice Make(IHoneybadgerClient client, 
        Exception error, 
        Dictionary<string, object>? hbContext = null)
    {
        // When creating a StackTrace from an exception which does not have a StackTrace, we skip the first frame,
        // which is the frame of the NoticeFactory.Make method.
        var stackTrace = error.StackTrace == null 
            ? new StackTrace(1, true) 
            : new StackTrace(error, 0, true);

        return Make(client, error.Message, stackTrace, error.GetType().FullName, hbContext);
    }
    
    private Notice Make(IHoneybadgerClient client, string message, StackTrace stackTrace,
        string? className = null, Dictionary<string, object>? hbContext = null)
    {
        var notice = new Notice(
            BreadcrumbsFactory.Get(client),
            ErrorFactory.Get(stackTrace, message, className, client.Options.ProjectRoot),
            NotifierFactory.Get(),
            RequestFactory.Get(hbContext),
            ServerFactory.Get(client)
        )
        {
            Details = null, // todo
        };
        EnrichNotice(client, notice);
        
        return notice;
    }
}