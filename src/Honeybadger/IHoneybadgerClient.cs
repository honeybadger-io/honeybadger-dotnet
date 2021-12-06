using Honeybadger.Schema;

namespace Honeybadger;

public interface IHoneybadgerClient
{
    public void Notify(Notice notice);
}