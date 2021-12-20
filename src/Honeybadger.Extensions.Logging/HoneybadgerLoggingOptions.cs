using Microsoft.Extensions.Logging;

namespace Honeybadger.Extensions.Logging;

public class HoneybadgerLoggingOptions : HoneybadgerOptions
{
    /// <summary>
    /// Logs with this level or higher will be stored as breadcrumbs
    /// </summary>
    public LogLevel MinimumBreadcrumbLevel { get; set; } = LogLevel.Information;

    /// <summary>
    /// Logs with this level or higher will be reported to Honeybadger
    /// </summary>
    public LogLevel MinimumNoticeLevel { get; set; } = LogLevel.Error;
}