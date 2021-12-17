using System.Text.Json.Serialization;

namespace Honeybadger.Schema;

public class Request
{
    [JsonPropertyName("action")]
    public string? Action { get; set; }

    [JsonPropertyName("cgi_data")]
    public CgiData? CgiData { get; set; }

    [JsonPropertyName("component")]
    public string? Component { get; set; }

    [JsonPropertyName("context")]
    public Dictionary<string, object>? Context { get; set; }

    [JsonPropertyName("params")]
    public Dictionary<string, object>? Params { get; set; }

    [JsonPropertyName("session")]
    public Dictionary<string, object>? Session { get; set; }

    [JsonPropertyName("url")]
    public string? Url { get; set; }
}

public class CgiData
{
    [JsonPropertyName("HTTP_COOKIE")] 
    public string? HttpCookie { get; set; }

    [JsonPropertyName("HTTP_HOST")]
    public string? HttpHost { get; set; }

    [JsonPropertyName("HTTP_REFERER")]
    public string? HttpReferer { get; set; }

    [JsonPropertyName("HTTP_USER_AGENT")]
    public string? HttpUserAgent { get; set; }

    [JsonPropertyName("HTTP_X_FORWARDED_FOR")]
    public string? HttpXForwardedFor { get; set; }

    [JsonPropertyName("REMOTE_ADDR")]
    public string? RemoteAddr { get; set; }

    [JsonPropertyName("REQUEST_METHOD")]
    public string? RequestMethod { get; set; }
}