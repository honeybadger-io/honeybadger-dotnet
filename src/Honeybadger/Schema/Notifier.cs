using System.Text.Json.Serialization;

namespace Honeybadger.Schema;

public class Notifier
{
    [JsonPropertyName("name")]
    public string Name { get; }

    [JsonPropertyName("url")]
    public string Url { get; }

    [JsonPropertyName("version")]
    public string Version { get; }

    public Notifier(string name, string version, string url)
    {
        Name = name;
        Version = version;
        Url = url;
    }
}