using Honeybadger.Schema;

namespace Honeybadger;

public class NullClient : IHoneybadgerClient
{
    public HoneybadgerOptions Options { get; }
    
    public void Notify(string message)
    {
    }

    public void Notify(string message, Dictionary<string, object> context)
    {
    }

    public void Notify(Exception error)
    {
    }

    public void Notify(Exception error, Dictionary<string, object> context)
    {
    }

    public void AddContext(Dictionary<string, object> context)
    {
    }

    public void ResetContext()
    {
    }

    public void AddBreadcrumb(string message, string category, Dictionary<string, object> options)
    {
    }

    public void ResetBreadcrumbs()
    {
    }
    
    Trail[]? IHoneybadgerClient.GetBreadcrumbs()
    {
        return null;
    }
}