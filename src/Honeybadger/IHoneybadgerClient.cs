namespace Honeybadger;

public interface IHoneybadgerClient
{
    public void Notify(HoneybadgerNotice notice);
}