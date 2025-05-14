using System.Diagnostics;
using Honeybadger.Schema;

namespace Honeybadger.NoticeHelpers;

public static class ErrorFactory
{
    public static Error Get(StackTrace stackTrace, string message, string? className = null)
    {
        var @class = className ?? stackTrace.GetFrame(0)?.GetMethod()?.DeclaringType?.FullName ?? "CLASS";
        return new Error(@class, message, GetBacktraces(stackTrace))
        {
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
                Context = DetermineContext(sf.GetFileName(), HoneybadgerClient.Current?.Options?.ProjectRoot),
            });
        }

        return result.ToArray();
    }

    private static string DetermineContext(string? fileName, string? projectRoot)
    {
        if (!string.IsNullOrEmpty(projectRoot) &&
            !string.IsNullOrEmpty(fileName) &&
            fileName.StartsWith(projectRoot))
        {
            return "app";
        }

        return "all";
    }
}
