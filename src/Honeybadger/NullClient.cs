using Honeybadger.Schema;

namespace Honeybadger;

public class NullClient: IHoneybadgerClient
{
    public void Notify(Notice notice)
    {
        
    }
}