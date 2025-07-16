using Honeybadger.Schema;

namespace Honeybadger.NoticeHelpers;

public static class RequestFactory
{
    public static Request Get(Dictionary<string, object>? context = null)
    {
        return new Request
        {
            Context = context,
            CgiData = null,
            Action = null,
            Component = null,
            Params = null,
            Session = null,
            Url = null
        };
    }
}