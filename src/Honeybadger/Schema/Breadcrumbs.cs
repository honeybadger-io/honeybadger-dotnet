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
    public string? Category { get; set; }

    [JsonPropertyName("message")]
    public string? Message { get; set; }

    [JsonPropertyName("metadata")]
    public Dictionary<string, Metadatum>? Metadata { get; set; }

    /// <summary>
    /// iso8601 formatted with milliseconds
    /// </summary>
    [JsonPropertyName("timestamp")]
    public DateTimeOffset Timestamp { get; set; }
}

public struct Metadatum
{
    public bool? Bool;
    public double? Double;
    public string? String;

    public static implicit operator Metadatum(bool boolValue) => new Metadatum { Bool = boolValue };
    public static implicit operator Metadatum(double doubleValue) => new Metadatum { Double = doubleValue };
    public static implicit operator Metadatum(string? stringValue) => new Metadatum { String = stringValue };
    public bool IsNull => Bool == null && Double == null && String == null;
}