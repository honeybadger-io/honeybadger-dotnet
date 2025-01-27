namespace Honeybadger;

public class HoneybadgerOptions
{
    /// <summary>
    /// Required. The project's private API key.
    /// </summary>
    public string ApiKey { get; set; }

    /// <summary>
    /// The path to the project's executable code.
    /// </summary>
    public string? ProjectRoot { get; set; } = null;

    /// <summary>
    /// The environment name of the application.
    /// </summary>
    public string? AppEnvironment { get; set; } = null;

    /// <summary>
    /// The hostname of the system.
    /// </summary>
    public string HostName { get; set; }

    /// <summary>
    /// The base API Endpoint
    /// </summary>
    public Uri Endpoint { get; set; } = new(Constants.DefaultApiEndpoint);

    /// <summary>
    /// A list of keys whose values are replaced with "[FILTERED]" in sensitive data objects (like request parameters).
    /// </summary>
    public string[] FilterKeys { get; set; } = Constants.DefaultFilterKeys;

    /// <summary>
    /// A list of development environments. When environment is in the list, log errors locally instead of reporting.
    /// </summary>
    public string[] DevelopmentEnvironments { get; set; } = Constants.DefaultDevelopmentEnvironments;

    /// <summary>
    /// Explicit override for development environments check (<see cref="DevelopmentEnvironments"/>).
    /// When true, always report errors.
    /// </summary>
    public bool ReportData { get; set; }

    /// <summary>
    /// The revision of the current deploy
    /// </summary>
    public string? Revision { get; set; }

    /// <summary>
    /// Allow/disallow breadcrumbs.
    /// </summary>
    public bool BreadcrumbsEnabled { get; set; } = true;

    /// <summary>
    /// Maximum number of breadcrumbs.
    /// </summary>
    public int MaxBreadcrumbs { get; set; } = 40;

    /// <summary>
    /// Mostly here to be utilized by unit tests.
    /// </summary>
    public HttpClient? HttpClient { get; set; }

    public HoneybadgerOptions()
    {
        ApiKey = Environment.GetEnvironmentVariable("HONEYBADGER_API_KEY") ?? "";
        ProjectRoot = Environment.GetEnvironmentVariable("HONEYBADGER_PROJECT_ROOT");
        AppEnvironment = (Environment.GetEnvironmentVariable("HONEYBADGER_APP_ENVIRONMENT") ??
                          Environment.GetEnvironmentVariable("ASPNETCORE_ENVIRONMENT") ?? "development").ToLower();
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

        ReportData = GetBoolFromEnv("HONEYBADGER_REPORT_DATA") ?? false;
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

    public bool ShouldReport()
    {
        if (string.IsNullOrEmpty(ApiKey))
        {
            return false;
        }
        
        if (ReportData)
        {
            return true;
        }
        
        return !DevelopmentEnvironments.Contains(AppEnvironment, StringComparer.InvariantCultureIgnoreCase);
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