namespace Honeybadger;

public class HoneybadgerOptions
{
    /// <summary>
    /// Required. The project's private API key.
    /// </summary>
    public string ApiKey { get; set; } = null!;

    /// <summary>
    /// The path to the project's executable code.
    /// </summary>
    public string? ProjectRoot { get; set; }

    /// <summary>
    /// The environment name of the application.
    /// </summary>
    public string? AppEnvironment { get; set; } = "Production";

    /// <summary>
    /// The hostname of the system.
    /// </summary>
    public string HostName { get; set; } = "";

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
    public string? Revision { get; set; } = null;

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
    
    /// <summary>
    /// Automatically report unhandled exceptions for
    /// .Net Core web applications by registering a middleware
    /// to catch unhandled exceptions.
    /// </summary>
    public bool ReportUnhandledExceptions { get; set; } = true; 
    
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
}