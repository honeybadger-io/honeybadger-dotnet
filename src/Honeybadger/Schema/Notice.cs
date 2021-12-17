using System.Text.Json.Serialization;

namespace Honeybadger.Schema;

public class Notice
{
    [JsonPropertyName("breadcrumbs")]
    public Breadcrumbs? Breadcrumbs { get; set; }

    [JsonPropertyName("details")]
    public Dictionary<string, Dictionary<string, object>>? Details { get; set; }

    [JsonPropertyName("error")]
    public Error? Error { get; set; }

    [JsonPropertyName("notifier")]
    public Notifier? Notifier { get; set; }

    [JsonPropertyName("request")]
    public Request? Request { get; set; }

    [JsonPropertyName("server")]
    public Server? Server { get; set; }
}