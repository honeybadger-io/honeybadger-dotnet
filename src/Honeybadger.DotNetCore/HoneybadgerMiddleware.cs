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
        context.Items[HttpContextItemsKey] = client;
        
        // capture the request information now as the http context
        // may be changed by other error handlers after an exception
        // has occurred
        // var bugsnagRequestInformation = context.ToRequest();

        // client.BeforeNotify(report => {
        //     report.Event.Request = bugsnagRequestInformation;
        // });

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
            client.Notify(exception);
            throw;
        }
    }
}