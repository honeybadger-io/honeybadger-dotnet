using System.Reflection;
using Honeybadger.Schema;

namespace Honeybadger.NoticeHelpers;

public static class NotifierFactory
{
    private const string GithubUrl = "https://github.com/subzero10/honeybadger-dotnet";
    public static Notifier Get()
    {
        return new Notifier
        {
            Name = Assembly.GetCallingAssembly().GetName().Name,
            Url = GithubUrl,
            Version = Assembly.GetCallingAssembly().GetName().Version?.ToString()
        };
    }
}