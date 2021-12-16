using Honeybadger.Schema;

namespace Honeybadger;

public class NullClient: IHoneybadgerClient
{
    public HoneybadgerOptions Options { get; }
    
    public void Notify(Notice notice)
    {
    }

    public void Notify(string message)
    {
    }

    public void Notify(Exception error)
    {
    }
}