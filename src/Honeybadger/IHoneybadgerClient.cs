using Honeybadger.Schema;

namespace Honeybadger;

public interface IHoneybadgerClient
{
    public void Notify(string message);
    public void Notify(string message, Dictionary<string, object> context);
    public void Notify(Exception error);
    public void Notify(Exception error, Dictionary<string, object> context);
    public Task NotifyAsync(string message);
    public Task NotifyAsync(string message, Dictionary<string, object> context);
    public Task NotifyAsync(Exception error);
    public Task NotifyAsync(Exception error, Dictionary<string, object> context);
    public void AddContext(Dictionary<string, object> context);
    public void ResetContext();
    public void AddBreadcrumb(string message, string category, Dictionary<string, object?>? options = null);
    public void ResetBreadcrumbs();
    public HoneybadgerOptions Options { get; }

    internal Trail[]? GetBreadcrumbs(); }