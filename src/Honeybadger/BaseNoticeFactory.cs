using Honeybadger.Schema;

namespace Honeybadger;

public class BaseNoticeFactory : NoticeFactory
{
    public override void EnrichNotice(IHoneybadgerClient client, Notice notice)
    {
        // no-op
    }
}