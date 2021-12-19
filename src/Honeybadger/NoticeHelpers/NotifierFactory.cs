using System.Reflection;
using Honeybadger.Schema;

namespace Honeybadger.NoticeHelpers;

public static class NotifierFactory
{
    public static Notifier Get()
    {
        var assembly = Assembly.GetCallingAssembly().GetName();
        return new Notifier(
            assembly.Name ?? "ASSEMBLY",
            assembly.Version?.ToString() ?? "VERSION",
            Constants.GithubUrl
        );
    }
}