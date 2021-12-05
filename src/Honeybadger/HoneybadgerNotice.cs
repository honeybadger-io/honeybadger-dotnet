namespace Honeybadger;

public class HoneybadgerNotice
{
    private readonly Exception? _exception;

    public HoneybadgerNotice(Exception? exception)
    {
        _exception = exception;
    }
}