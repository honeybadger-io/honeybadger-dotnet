using Honeybadger.Schema;

namespace Honeybadger.NoticeHelpers;

public static class BreadcrumbsFactory
{
    public static Breadcrumbs Get(IHoneybadgerClient client)
    {
        return new Breadcrumbs
        {
            Enabled = client.Options.BreadcrumbsEnabled,
            Trail = client.GetBreadcrumbs() ?? Array.Empty<Trail>()
        };
    }
}