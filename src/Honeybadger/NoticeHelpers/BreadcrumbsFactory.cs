using Honeybadger.Schema;

namespace Honeybadger.NoticeHelpers;

public class BreadcrumbsFactory
{
    public static Breadcrumbs Get()
    {
        return new Breadcrumbs
        {
            Enabled = false
        };
    }
}