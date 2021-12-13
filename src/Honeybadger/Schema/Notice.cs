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

public class Breadcrumbs
{
    [JsonPropertyName("enabled")]
    public bool Enabled { get; set; }

    [JsonPropertyName("trail")]
    public Trail[]? Trail { get; set; }
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

public class Error
{
    [JsonPropertyName("backtrace")]
    public ErrorBacktrace[]? Backtrace { get; set; }

    [JsonPropertyName("causes")]
    public Cause[]? Causes { get; set; }

    [JsonPropertyName("class")]
    public string? Class { get; set; }

    [JsonPropertyName("fingerprint")]
    public string? Fingerprint { get; set; }

    [JsonPropertyName("message")]
    public string? Message { get; set; }

    [JsonPropertyName("source")]
    public ErrorSource? Source { get; set; }

    [JsonPropertyName("tags")]
    public string[]? Tags { get; set; }
}

public class ErrorBacktrace
{
    [JsonPropertyName("args")]
    public Arg[]? Args { get; set; }

    [JsonPropertyName("class")]
    public string? Class { get; set; }

    [JsonPropertyName("column")]
    public Column? Column { get; set; }

    [JsonPropertyName("context")]
    public Context? Context { get; set; }

    [JsonPropertyName("file")]
    public string? File { get; set; }

    [JsonPropertyName("method")]
    public string? Method { get; set; }

    [JsonPropertyName("number")]
    public Column Number { get; set; }

    [JsonPropertyName("source")]
    public BacktraceSource? Source { get; set; }

    [JsonPropertyName("type")]
    public string? Type { get; set; }
}

public class BacktraceSource
{
    [JsonPropertyName("1")]
    public string? The1 { get; set; }
}

public class Cause
{
    [JsonPropertyName("backtrace")]
    public CauseBacktrace[]? Backtrace { get; set; }

    [JsonPropertyName("class")]
    public string? Class { get; set; }

    [JsonPropertyName("message")]
    public string? Message { get; set; }
}

public class CauseBacktrace
{
    [JsonPropertyName("class")]
    public string? Class { get; set; }

    [JsonPropertyName("column")]
    public Column? Column { get; set; }

    [JsonPropertyName("context")]
    public Context? Context { get; set; }

    [JsonPropertyName("file")]
    public string? File { get; set; }

    [JsonPropertyName("method")]
    public string? Method { get; set; }

    [JsonPropertyName("number")]
    public Column Number { get; set; }

    [JsonPropertyName("type")]
    public string? Type { get; set; }
}

public class ErrorSource
{
    [JsonPropertyName("1")]
    public string? The1 { get; set; }
}

public class Notifier
{
    [JsonPropertyName("name")]
    public string? Name { get; set; }

    [JsonPropertyName("url")]
    public string? Url { get; set; }

    [JsonPropertyName("version")]
    public string? Version { get; set; }
}

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
    [JsonPropertyName("HTTP_COOKIE")] public string? HttpCookie { get; set; }

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

public class Server
{
    [JsonPropertyName("environment_name")]
    public string? EnvironmentName { get; set; }

    [JsonPropertyName("hostname")]
    public string? Hostname { get; set; }

    [JsonPropertyName("pid")]
    public long? Pid { get; set; }

    [JsonPropertyName("project_root")]
    public string? ProjectRoot { get; set; }

    [JsonPropertyName("revision")]
    public string? Revision { get; set; }

    [JsonPropertyName("stats")]
    public Stats? Stats { get; set; }

    [JsonPropertyName("time")]
    public string? Time { get; set; }
}

public class Stats
{
    [JsonPropertyName("load")]
    public Load? Load { get; set; }

    [JsonPropertyName("mem")]
    public Mem? Mem { get; set; }
}

public class Load
{
    [JsonPropertyName("fifteen")]
    public double Fifteen { get; set; }

    [JsonPropertyName("five")]
    public double Five { get; set; }

    [JsonPropertyName("one")]
    public double One { get; set; }
}

public class Mem
{
    [JsonPropertyName("buffers")]
    public double Buffers { get; set; }

    [JsonPropertyName("cached")]
    public double Cached { get; set; }

    [JsonPropertyName("free")]
    public double Free { get; set; }

    [JsonPropertyName("free_total")]
    public double FreeTotal { get; set; }

    [JsonPropertyName("total")]
    public double Total { get; set; }
}

public enum Context { All, App };

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

public struct Arg
{
    public object[] AnythingArray;
    public Dictionary<string, object> AnythingMap;
    public bool? Bool;
    public double? Double;
    public string? String;

    public static implicit operator Arg(object[] anythingArray) => new Arg { AnythingArray = anythingArray };
    public static implicit operator Arg(Dictionary<string, object> anythingMap) => new Arg { AnythingMap = anythingMap };
    public static implicit operator Arg(bool boolValue) => new Arg { Bool = boolValue };
    public static implicit operator Arg(double doubleValue) => new Arg { Double = doubleValue };
    public static implicit operator Arg(string? stringValue) => new Arg { String = stringValue };
    public bool IsNull => AnythingArray == null && Bool == null && Double == null && AnythingMap == null && String == null;
}

public struct Column
{
    public long? Integer;
    public string? String;

    public static implicit operator Column(long integerValue) => new Column { Integer = integerValue };
    public static implicit operator Column(string? stringValue) => new Column { String = stringValue };
}