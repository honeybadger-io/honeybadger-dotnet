using Honeybadger.Schema;

namespace Honeybadger;

public interface IHoneybadgerClient
{
    public void Notify(Notice notice);
    public void Notify(string message);
    public void Notify(Exception error);
}