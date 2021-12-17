using Honeybadger.Schema;

namespace Honeybadger;

public class NullClient: IHoneybadgerClient
{
    public HoneybadgerOptions Options { get; }
    
    public void Notify(string message, Dictionary<string, object>? context = null)
    {
    }

    public void Notify(Exception error, Dictionary<string, object>? context = null)
    {
    }

    public void AddContext(Dictionary<string, object> context)
    {
    }

    public void ResetContext()
    {
    }
}