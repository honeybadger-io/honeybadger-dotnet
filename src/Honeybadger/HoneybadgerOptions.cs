namespace Honeybadger;

public class HoneybadgerOptions
{
    public bool IsDisabled { get; set; } = false;
    
    public string ApiEndpoint { get; set; }
    
    public string ApiKey { get; }

    public HoneybadgerOptions(string apiKey)
    {
        ApiKey = apiKey;
    }
}