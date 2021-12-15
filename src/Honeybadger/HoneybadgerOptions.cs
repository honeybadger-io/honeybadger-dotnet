namespace Honeybadger;

public class HoneybadgerOptions
{
    /// <summary>
    /// Required. The project's private API key.
    /// </summary>
    public string ApiKey { get; init; }

    /// <summary>
    /// The path to the project's executable code.
    /// </summary>
    public string? ProjectRoot { get; init; } = null;

    /// <summary>
    /// The environment name of the application.
    /// </summary>
    public string? AppEnvironment { get; init; } = null;

    /// <summary>
    /// The hostname of the system.
    /// </summary>
    public string HostName { get; init; } = "hostname";
    
    /// <summary>
    /// The base API Endpoint
    /// </summary>
    public Uri Endpoint { get; init; } = new Uri("https://api.honeybadger.io");

    /// <summary>
    /// A list of keys whose values are replaced with "[FILTERED]" in sensitive data objects (like request parameters).
    /// </summary>
    public string[] FilterKeys { get; init; } = {"password", "password_confirmation", "credit_card"};

    /// <summary>
    /// A list of development environments. When environment is in the list, log errors locally instead of reporting.
    /// </summary>
    public string[] DevelopmentEnvironments { get; init; } = {"test", "development"};
    
    /// <summary>
    /// Explicit override for development environments check; when true, always report errors.
    /// </summary>
    public bool ReportData { get; init; }

    /// <summary>
    /// The revision of the current deploy
    /// </summary>
    public string? Revision { get; init; }

    public HoneybadgerOptions()
    {
        ApiKey = Environment.GetEnvironmentVariable("HONEYBADGER_API_KEY") ?? "";
        ProjectRoot = Environment.GetEnvironmentVariable("HONEYBADGER_PROJECT_ROOT");
        AppEnvironment = Environment.GetEnvironmentVariable("HONEYBADGER_APP_ENVIRONMENT");
        HostName = Environment.GetEnvironmentVariable("HONEYBADGER_HOSTNAME") ?? "hostname";
        var endpoint = Environment.GetEnvironmentVariable("HONEYBADGER_ENDPOINT");
        if (endpoint != null)
        {
            Endpoint = new Uri(endpoint);
        }

        var filterKeys = GetArrayFromEnv("HONEYBADGER_FILTER_KEYS");
        if (filterKeys != null)
        {
            FilterKeys = filterKeys;
        }
        
        var devEnvironments = GetArrayFromEnv("HONEYBADGER_DEVELOPMENT_ENVIRONMENTS");
        if (devEnvironments != null)
        {
            DevelopmentEnvironments = devEnvironments;
        }

        var reportDataUseEnv = false;
        var reportDataLocal = false;
        var reportDataStr = Environment.GetEnvironmentVariable("HONEYBADGER_REPORT_DATA");
        if (reportDataStr != null)
        {
            reportDataUseEnv = bool.TryParse(reportDataStr, out reportDataLocal);
        }
        ReportData = reportDataUseEnv ? reportDataLocal : !DevelopmentEnvironments.Contains(AppEnvironment);

        Revision = Environment.GetEnvironmentVariable("HONEYBADGER_REVISION");
    }

    public HoneybadgerOptions(string apiKey) : this()
    {
        ApiKey = apiKey;
    }

    private static string[]? GetArrayFromEnv(string envName)
    {
        var result = Environment.GetEnvironmentVariable(envName);
        return result?.Split(',').Select(i => i.Trim()).ToArray();
    }
}