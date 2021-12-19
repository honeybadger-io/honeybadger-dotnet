using System.Text.Json.Serialization;

namespace Honeybadger.Schema;

public class Error
{
    [JsonPropertyName("backtrace")]
    public ErrorBacktrace[] Backtrace { get; set; }
    
    [JsonPropertyName("class")]
    public string Class { get; set; }

    [JsonPropertyName("fingerprint")]
    public string? Fingerprint { get; set; }

    [JsonPropertyName("message")]
    public string Message { get; set; }
    
    [JsonPropertyName("tags")]
    public string[]? Tags { get; set; }

    public Error(string @class, string message, ErrorBacktrace[] backtrace)
    {
        Class = @class;
        Message = message;
        Backtrace = backtrace;
    }
}

public class ErrorBacktrace
{
    [JsonPropertyName("class")]
    public string? Class { get; set; }

    [JsonPropertyName("column")]
    public string? Column { get; set; }

    [JsonPropertyName("context")]
    public Context? Context { get; set; }

    [JsonPropertyName("file")]
    public string? File { get; set; }

    [JsonPropertyName("method")]
    public string? Method { get; set; }

    [JsonPropertyName("number")]
    public string? Number { get; set; }
    
    /*
    [JsonPropertyName("args")]
    public object[]? Args { get; set; }
    
    [JsonPropertyName("source")]
    public object? Source { get; set; }
    
    [JsonPropertyName("type")]
    public string? Type { get; set; }
    */
}

public enum Context { All, App };