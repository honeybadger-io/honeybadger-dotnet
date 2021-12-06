using Honeybadger.Schema;

namespace Honeybadger;

public static class NoticeFactory
{
    public static Notice Make(string message)
    {
        var notice = new Notice
        {
            Error = new Error
            {
                Message = message
            }
        };

        return notice;
    }
    
    public static Notice Make(Exception error)
    {
        var notice = new Notice
        {
            Error = new Error
            {
                Message = error.Message
            }
        };

        return notice;
    }
}