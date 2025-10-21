using Honeybadger.Schema;

namespace Honeybadger;

/// <summary>
/// Main interface for reporting errors to Honeybadger.
/// </summary>
public interface IHoneybadgerClient
{
    /// <summary>
    /// Report a message to Honeybadger.
    /// </summary>
    /// <param name="message">The message to report.</param>
    public void Notify(string message);

    /// <summary>
    /// Report a message to Honeybadger with additional context.
    /// </summary>
    /// <param name="message">The message to report.</param>
    /// <param name="context">Additional context data. Special keys:
    /// - "tags": string or string[] - Tags to categorize the error
    /// - "fingerprint": string - Custom fingerprint for grouping errors</param>
    public void Notify(string message, Dictionary<string, object> context);

    /// <summary>
    /// Report an exception to Honeybadger.
    /// </summary>
    /// <param name="error">The exception to report.</param>
    public void Notify(Exception error);

    /// <summary>
    /// Report an exception to Honeybadger with additional context.
    /// </summary>
    /// <param name="error">The exception to report.</param>
    /// <param name="context">Additional context data. Special keys:
    /// - "tags": string or string[] - Tags to categorize the error
    /// - "fingerprint": string - Custom fingerprint for grouping errors</param>
    public void Notify(Exception error, Dictionary<string, object> context);

    /// <summary>
    /// Report a pre-constructed Notice to Honeybadger.
    /// </summary>
    /// <param name="notice">The notice to report.</param>
    public void Notify(Notice notice);

    /// <summary>
    /// Asynchronously report a message to Honeybadger.
    /// </summary>
    /// <param name="message">The message to report.</param>
    public Task NotifyAsync(string message);

    /// <summary>
    /// Asynchronously report a message to Honeybadger with additional context.
    /// </summary>
    /// <param name="message">The message to report.</param>
    /// <param name="context">Additional context data. Special keys:
    /// - "tags": string or string[] - Tags to categorize the error
    /// - "fingerprint": string - Custom fingerprint for grouping errors</param>
    public Task NotifyAsync(string message, Dictionary<string, object> context);

    /// <summary>
    /// Asynchronously report an exception to Honeybadger.
    /// </summary>
    /// <param name="error">The exception to report.</param>
    public Task NotifyAsync(Exception error);

    /// <summary>
    /// Asynchronously report an exception to Honeybadger with additional context.
    /// </summary>
    /// <param name="error">The exception to report.</param>
    /// <param name="context">Additional context data. Special keys:
    /// - "tags": string or string[] - Tags to categorize the error
    /// - "fingerprint": string - Custom fingerprint for grouping errors</param>
    public Task NotifyAsync(Exception error, Dictionary<string, object> context);

    /// <summary>
    /// Asynchronously report a pre-constructed Notice to Honeybadger.
    /// </summary>
    /// <param name="notice">The notice to report.</param>
    public Task NotifyAsync(Notice notice);

    /// <summary>
    /// Add context data that will be included with all subsequent error reports.
    /// Context is stored per-thread and will not affect other threads.
    /// </summary>
    /// <param name="context">Context data to add. Special keys:
    /// - "tags": string or string[] - Tags to categorize errors
    /// - "fingerprint": string - Custom fingerprint for grouping errors</param>
    public void AddContext(Dictionary<string, object> context);

    /// <summary>
    /// Get the current context, optionally merged with additional context.
    /// </summary>
    /// <param name="context">Optional additional context to merge with the current context.</param>
    /// <returns>The merged context dictionary.</returns>
    public Dictionary<string, object> GetContext(Dictionary<string, object>? context = null);

    /// <summary>
    /// Clear all context data for the current thread.
    /// </summary>
    public void ResetContext();

    /// <summary>
    /// Add a breadcrumb to track events leading up to an error.
    /// Breadcrumbs are stored per-thread.
    /// </summary>
    /// <param name="message">The breadcrumb message.</param>
    /// <param name="category">The breadcrumb category (e.g., "navigation", "request").</param>
    /// <param name="options">Optional metadata for the breadcrumb.</param>
    public void AddBreadcrumb(string message, string category, Dictionary<string, object?>? options = null);

    /// <summary>
    /// Clear all breadcrumbs for the current thread.
    /// </summary>
    public void ResetBreadcrumbs();

    /// <summary>
    /// Gets the current Honeybadger configuration options.
    /// </summary>
    public HoneybadgerOptions Options { get; }

    /// <summary>
    /// Configure Honeybadger with new options.
    /// </summary>
    /// <param name="options">The configuration options to apply.</param>
    public void Configure(HoneybadgerOptions options);

    /// <summary>
    /// Internal method to retrieve breadcrumbs for the current thread.
    /// </summary>
    internal Trail[]? GetBreadcrumbs();
}