namespace Honeybadger;

public static class Constants
{
    public const string DateTimeFormat = "yyyy-MM-ddTHH:mm:ssZ";
    public const string GithubUrl = "https://github.com/honeybadger-io/honeybadger-dotnet";
    public const string DefaultApiEndpoint = "https://api.honeybadger.io";
    public const string DefaultHostname = "hostname";
    public const string ContextTagsKey = "tags";
    public const string ContextFingerprintKey = "fingerprint";

    public static readonly string[] DefaultFilterKeys =
    {
        "password", "password_confirmation", "credit_card"
    };

    public static readonly string[] DefaultDevelopmentEnvironments = {"test", "development", "dev"};
}