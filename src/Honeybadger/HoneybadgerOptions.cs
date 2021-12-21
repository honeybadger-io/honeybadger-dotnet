namespace Honeybadger;

public class HoneybadgerOptions
{
    /// <summary>
    /// Required. The project's private API key.
    /// </summary>
    public string ApiKey { get; }

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
    public string HostName { get; init; }
    
    /// <summary>
    /// The base API Endpoint
    /// </summary>
    public Uri Endpoint { get; init; } = new(Constants.DefaultApiEndpoint);

    /// <summary>
    /// A list of keys whose values are replaced with "[FILTERED]" in sensitive data objects (like request parameters).
    /// </summary>
    public string[] FilterKeys { get; init; } = Constants.DefaultFilterKeys;

    /// <summary>
    /// A list of development environments. When environment is in the list, log errors locally instead of reporting.
    /// </summary>
    public string[] DevelopmentEnvironments { get; init; } = Constants.DefaultDevelopmentEnvironments;
    
    /// <summary>
    /// Explicit override for development environments check; when true, always report errors.
    /// </summary>
    public bool ReportData { get; init; }

    /// <summary>
    /// The revision of the current deploy
    /// </summary>
    public string? Revision { get; init; }

    /// <summary>
    /// Allow/disallow breadcrumbs.
    /// </summary>
    public bool BreadcrumbsEnabled { get; init; } = true;

    /// <summary>
    /// Maximum number of breadcrumbs.
    /// </summary>
    public int MaxBreadcrumbs { get; init; } = 40;

    public HoneybadgerOptions()
    {
        ApiKey = Environment.GetEnvironmentVariable("HONEYBADGER_API_KEY") ?? "";
        ProjectRoot = Environment.GetEnvironmentVariable("HONEYBADGER_PROJECT_ROOT");
        AppEnvironment = (Environment.GetEnvironmentVariable("HONEYBADGER_APP_ENVIRONMENT") ?? "development").ToLower();
        HostName = Environment.GetEnvironmentVariable("HONEYBADGER_HOSTNAME") ?? Constants.DefaultHostname;
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

        ReportData = GetBoolFromEnv("HONEYBADGER_REPORT_DATA") ?? !DevelopmentEnvironments.Contains(AppEnvironment);
        Revision = Environment.GetEnvironmentVariable("HONEYBADGER_REVISION");

        var breadcrumbsEnabled = GetBoolFromEnv("HONEYBADGER_BREADCRUMBS_ENABLED");
        if (breadcrumbsEnabled.HasValue)
        {
            BreadcrumbsEnabled = breadcrumbsEnabled.Value;
        }

        var maxBreadcrumbs = GetIntFromEnv("HONEYBADGER_MAX_BREADCRUMBS");
        if (maxBreadcrumbs.HasValue)
        {
            MaxBreadcrumbs = maxBreadcrumbs.Value;
        }
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

    private static bool? GetBoolFromEnv(string envName)
    {
        var readFromEnv = false;
        var envValue = false;
        var envValueStr = Environment.GetEnvironmentVariable(envName);
        if (envValueStr != null)
        {
            readFromEnv = bool.TryParse(envValueStr, out envValue);
        }

        return readFromEnv ? envValue : null;
    }

    private static int? GetIntFromEnv(string envName)
    {
        var readFromEnv = false;
        var envValue = 0;
        var envValueStr = Environment.GetEnvironmentVariable(envName);
        if (envValueStr != null)
        {
            readFromEnv = int.TryParse(envValueStr, out envValue);
        }

        return readFromEnv ? envValue : null;
    } 
}