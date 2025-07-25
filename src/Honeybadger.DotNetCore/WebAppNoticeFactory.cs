using Honeybadger.Schema;
using Microsoft.AspNetCore.Http;

namespace Honeybadger.DotNetCore;

public class WebAppNoticeFactory : NoticeFactory
{
    private readonly IHttpContextAccessor _httpContextAccessor;
    
    public WebAppNoticeFactory(IHttpContextAccessor httpContextAccessor)
    {
        _httpContextAccessor = httpContextAccessor;
    }

    protected override void EnrichNotice(IHoneybadgerClient client, Notice notice)
    {
        var context = client.GetContext();
        var requestData = NoticeHelpers.RequestFactory.Get(_httpContextAccessor.HttpContext, context, client.Options);
        notice.Request = requestData;
    }
}