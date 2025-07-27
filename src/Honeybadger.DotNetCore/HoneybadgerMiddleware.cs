using Microsoft.AspNetCore.Http;

namespace Honeybadger.DotNetCore;

public class HoneybadgerMiddleware
{
    public const string HttpContextItemsKey = "Honeybadger.Client.DotNetCore";
    
    private readonly RequestDelegate _next;

    public HoneybadgerMiddleware(RequestDelegate requestDelegate)
    {
        _next = requestDelegate;
    }

    public async Task Invoke(HttpContext context, IHoneybadgerClient client)
    {
        // needed for HoneybadgerStartupFilter.cs
        context.Items[HttpContextItemsKey] = client;
        
        if (!client.Options.ShouldReport())
        {
            await _next(context);
            return;
        }

        try
        {
            if (client.Options.CaptureRequestBody)
            {
                context.Request.EnableBuffering();
            }
            
            await _next(context);
        }
        catch (Exception exception)
        {
            if (client.Options.ShouldReport() &&
                client.Options.ReportUnhandledExceptions)
            {
                // we don't want to block execution, so no await here
                _ = client.NotifyAsync(exception).ConfigureAwait(false);
            }
            throw;
        }
    }
}