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
        
        if (!client.Options.ReportData)
        {
            await _next(context);
            return;
        }

        try
        {
            await _next(context);
        }
        catch (Exception exception)
        {
            await client.NotifyAsync(exception);
            throw;
        }
    }
}