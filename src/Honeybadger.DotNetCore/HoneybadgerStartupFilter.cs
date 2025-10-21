using System.Diagnostics;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.DiagnosticAdapter;

namespace Honeybadger.DotNetCore;

public class HoneybadgerStartupFilter : IStartupFilter
{
    public Action<IApplicationBuilder> Configure(Action<IApplicationBuilder> next)
    {
        return builder =>
        {
            // catch exceptions when ASPNETCORE_ENVIRONMENT is Development
            builder.ApplicationServices.GetService<DiagnosticListener>()?.SubscribeWithAdapter(new DiagnosticSubscriber());
            
            builder.UseMiddleware<HoneybadgerMiddleware>();
            next(builder);
        };
    }
    
    private class DiagnosticSubscriber
    {
        /// <summary>
        /// Handles exceptions that the Microsoft.AspNetCore.Diagnostics.ExceptionHandlerMiddleware
        /// swallows.
        /// </summary>
        [DiagnosticName("Microsoft.AspNetCore.Diagnostics.HandledException")]
        public void OnHandledException(Exception exception, HttpContext httpContext)
        {
            LogException(exception, httpContext);
        }

        /// <summary>
        /// Handles exceptions that the Microsoft.AspNetCore.Diagnostics.DeveloperExceptionPageMiddleware
        /// swallows.
        /// </summary>
        [DiagnosticName("Microsoft.AspNetCore.Diagnostics.UnhandledException")]
        public void OnUnhandledException(Exception exception, HttpContext httpContext)
        {
            LogException(exception, httpContext);
        }

        private static void LogException(Exception exception, HttpContext httpContext)
        {
            httpContext.Items.TryGetValue(HoneybadgerMiddleware.HttpContextItemsKey, out var clientObject);
            
            if (clientObject is IHoneybadgerClient client && 
                client.Options.ShouldReport() && 
                client.Options.ReportUnhandledExceptions)
            {
                // no need to await, we're in a fire-and-forget context
                client.Notify(exception);
            }
        }
    }
}