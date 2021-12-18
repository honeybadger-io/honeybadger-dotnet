namespace Honeybadger;

public interface IHoneybadgerClient
{
    public void Notify(string message);
    public void Notify(string message, Dictionary<string, object> context);
    public void Notify(Exception error);
    public void Notify(Exception error, Dictionary<string, object> context);
    public void AddContext(Dictionary<string, object> context);
    public void ResetContext();
    public HoneybadgerOptions? Options { get; }
}