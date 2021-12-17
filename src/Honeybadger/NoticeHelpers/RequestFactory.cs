using Honeybadger.Schema;

namespace Honeybadger.NoticeHelpers;

public static class RequestFactory
{
    public static Request Get(Dictionary<string, object>? context = null)
    {
        return new Request
        {
            CgiData = GetCgiData(),
            Context = context,
            Action = null,
            Component = null,
            Params = null,
            Session = null,
            Url = null
        };
    }
    
    // todo: return type should not be nullable
    private static CgiData? GetCgiData()
    {
        return null;
    }
}