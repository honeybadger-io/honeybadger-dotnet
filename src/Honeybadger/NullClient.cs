using Honeybadger.Schema;

namespace Honeybadger;

public class NullClient: IHoneybadgerClient
{
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