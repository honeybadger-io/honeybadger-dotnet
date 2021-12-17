using System.Text.Json.Serialization;

namespace Honeybadger.Schema;

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