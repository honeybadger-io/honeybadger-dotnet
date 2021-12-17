using System.Reflection;
using Honeybadger.Schema;

namespace Honeybadger.NoticeHelpers;

public static class NotifierFactory
{
    public static Notifier Get()
    {
        return new Notifier
        {
            Name = Assembly.GetCallingAssembly().GetName().Name,
            Url = "https://github.com/honeybadger-io/honeybadger-dotnet",
            Version = Assembly.GetCallingAssembly().GetName().Version?.ToString()
        };
    }
}