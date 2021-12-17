using System.Diagnostics;
using Honeybadger.Schema;

namespace Honeybadger.NoticeHelpers;

public class ErrorFactory
{
    public static Error Get(StackTrace stackTrace, string message, string? className = null, object? context = null)
    {
        return new Error
        {
            Class = className ?? stackTrace.GetFrame(0)?.GetMethod()?.DeclaringType?.FullName,
            Message = message,
            Backtrace = GetBacktraces(stackTrace),
            Fingerprint = null, // todo
            Tags = null, // todo
        };
    }

    private static ErrorBacktrace[] GetBacktraces(StackTrace stackTrace)
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
                Column = sf.GetFileColumnNumber().ToString(),
                File = sf.GetFileName(),
                Number = sf.GetFileLineNumber().ToString(),
                Method = sf.GetMethod()?.Name,
                Context = null, // todo
            });
        }

        return result.ToArray();
    }
}