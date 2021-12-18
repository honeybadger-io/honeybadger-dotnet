using System.Text.Json.Serialization;

namespace Honeybadger.Schema;

public class Breadcrumbs
{
    [JsonPropertyName("enabled")]
    public bool Enabled { get; set; }

    [JsonPropertyName("trail")] 
    public Trail[] Trail { get; set; } = Array.Empty<Trail>();
}

public class Trail
{
    [JsonPropertyName("category")] 
    public string Category { get; set; } = "custom";

    [JsonPropertyName("message")]
    public string Message { get; set; }

    [JsonPropertyName("metadata")]
    public Dictionary<string, object>? Metadata { get; set; }

    /// <summary>
    /// iso8601 formatted with milliseconds
    /// </summary>
    [JsonPropertyName("timestamp")]
    public DateTimeOffset Timestamp { get; set; }

    public Trail(string message)
    {
        Message = message;
        Timestamp = DateTimeOffset.UtcNow;
    }
}