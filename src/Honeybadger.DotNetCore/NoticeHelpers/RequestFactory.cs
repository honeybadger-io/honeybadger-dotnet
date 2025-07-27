using System.Text;
using System.Text.Json;
using Honeybadger.DotNetCore.Utils;
using Honeybadger.Schema;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Http.Extensions; 
using Microsoft.AspNetCore.Routing;
using Microsoft.AspNetCore.WebUtilities;
using Microsoft.Net.Http.Headers;

namespace Honeybadger.DotNetCore.NoticeHelpers;

public static class RequestFactory
{
    private const int MaxStringLength = 1024;
    
    public static Request Get(
        HttpContext? httpContext = null,
        Dictionary<string, object>? hbContext = null,
        HoneybadgerOptions? options = null)
    {
        return new Request
        {
            Context = hbContext,   
            CgiData = GetCgiData(httpContext),
            Action = GetAction(httpContext, hbContext),
            Component = GetComponent(httpContext),
            Session = GetSession(httpContext, options?.FilterKeys),
            Url = FilterUrl(httpContext?.Request.GetEncodedUrl(), options?.FilterKeys),
            Params = (options?.CaptureRequestBody ?? false) 
                ? GetParams(httpContext, options?.FilterKeys) 
                : null,
        };
    }
    
    private static Dictionary<string, object>? FilterDictionary(Dictionary<string, object>? dictionary, string[]? filterKeys)
    {
        if (dictionary == null || filterKeys == null || filterKeys.Length == 0)
        {
            return dictionary;
        }

        var filtered = new Dictionary<string, object>();
        foreach (var kvp in dictionary)
        {
            switch (kvp.Value)
            {
                case Dictionary<string, object> nestedDict:
                {
                    var filteredDict = FilterDictionary(nestedDict, filterKeys);
                    filtered[kvp.Key] = JsonSerializer.Serialize(filteredDict);
                    break;
                }
                case object[] array:
                {
                    var value = string.Join(",", array.Select(v => v.ToString()));
                    filtered[kvp.Key] = FilterValue(kvp.Key, value, filterKeys);
                    break;
                }
                default:
                    filtered[kvp.Key] = FilterValue(kvp.Key, kvp.Value, filterKeys);
                    break;
            }
        }

        return filtered;
    }

    private static string? FilterUrl(string? url = null, string[]? filterKeys = null)
    {
        if (string.IsNullOrEmpty(url) || filterKeys == null || filterKeys.Length == 0)
        {
            return url;
        }

        try
        {
            var uri = new Uri(url);
            var queryParams = QueryHelpers.ParseNullableQuery(uri.Query);
            var filteredParams = queryParams?
                .Select(kvp =>
                {
                    var value = FilterValue(kvp.Key, kvp.Value, filterKeys);
                    return $"{Uri.EscapeDataString(kvp.Key)}={Uri.EscapeDataString(value)}";
                })
                .ToList() ?? new List<string>();

            var filteredQuery = filteredParams.Count > 0 ? "?" + string.Join("&", filteredParams) : "";
            return $"{uri.Scheme}://{uri.Authority}{uri.AbsolutePath}{filteredQuery}";
        }
        catch
        {
            return "[URL PARSING FAILED]";
        }
    }

    private static string FilterValue(string key, object value, string[] filterKeys)
    {
        var result = filterKeys.Any(filterKey => 
            string.Equals(key, filterKey, StringComparison.InvariantCultureIgnoreCase)) 
            ? "[FILTERED]" 
            : value.ToString() ?? string.Empty;

        // it's possible that we'll read files here, so we want to make sure we don't send them
        // to honeybadger. we can skip if string is too large
        return result.Length > MaxStringLength 
            ? "[TRIMMED]" 
            : result;
    }
    
    private static string? GetAction(HttpContext? httpContext, Dictionary<string, object>? hbContext)
    {
        if (hbContext != null && hbContext.TryGetValue("_action", out var hbContextAction))
        {
            return hbContextAction.ToString();
        }

        if (httpContext is null)
        {
            return null;
        }
        
        // For Minimal APIs, try to get the endpoint name
        var endpoint = httpContext.GetEndpoint();
        if (endpoint != null)
        {
            // Check for IEndpointNameMetadata first, then IRouteNameMetadata
            // This value will be set when .WithName() is used in Minimal APIs
            var name = endpoint.Metadata.GetMetadata<IEndpointNameMetadata>()?.EndpointName;
            if (name is not null)
            {
                return name;
            }

            name = endpoint.Metadata.GetMetadata<IRouteNameMetadata>()?.RouteName;
            if (name is not null)
            {
                return name;
            }

            // For Minimal APIs, the endpoint display name often contains the method name
            var displayName = endpoint.DisplayName;
            if (displayName is not null)
            {
                return displayName;
            }
        }

        // For MVC controllers, try to get from route data
        var routeData = httpContext.GetRouteData();
        if (routeData.Values.TryGetValue("action", out var routeAction))
        {
            return routeAction?.ToString();
        }

        return null;
    }

    private static string? GetComponent(HttpContext? httpContext)
    {
        if (httpContext == null)
        {
            return null;
        }
        
        // For MVC controllers, try to get controller name from route data
        var routeData = httpContext.GetRouteData();
        if (routeData.Values.TryGetValue("controller", out var controller))
        {
            return controller?.ToString();
        }

        // For Minimal APIs, return null
        return null;
    }
    
    /// <summary>
    /// This method assumes that HttpContext.Request.EnableBuffering()
    /// has already been called.
    /// It then reads the content of the body synchronously,
    /// but this should be OK, since the content most probably has already been read
    /// when binding to a request model.
    /// </summary>
    private static Dictionary<string, object>? GetParams(HttpContext? httpContext, string[]? filterKeys = null)
    {
        if (httpContext == null)
        {
            return null;
        }

        filterKeys ??= Array.Empty<string>();
        var parameters = new Dictionary<string, object>();
        
        // Get form data if present
        if (httpContext.Request.HasFormContentType)
        {
            var form =  httpContext.Request.Form
                .ToDictionary(k => k.Key, v => v.Value);
            foreach (var formParam in form)
            {
                parameters[formParam.Key] = FilterValue(formParam.Key, formParam.Value, filterKeys);
            }
        }
        
        // Get body data if present
        if (httpContext.Request.HasJsonContentType() 
            && httpContext.Request.Body is { CanRead: true, CanSeek: true })
        {
            StreamReader reader;
            var originalPosition = httpContext.Request.Body.Position;
            try
            {
                httpContext.Request.Body.Position = 0;
                reader = new StreamReader(httpContext.Request.Body,
                    Encoding.UTF8,
                    detectEncodingFromByteOrderMarks: true,
                    leaveOpen: true);

                // can't call .ReadToEnd() because sync calls to body will throw 
                var body = AsyncHelper.RunSync(() =>
                {
                    var result = reader.ReadToEndAsync();
                    reader.Dispose();
                    
                    return result;
                });
                if (body.Length > 0)
                {
                    var bodyParams = JsonSerializer.Deserialize<Dictionary<string, object>>(body);
                    var filteredBody = FilterDictionary(bodyParams, filterKeys);
                    if (filteredBody != null)
                    {
                        foreach (var kvp in filteredBody)
                        {
                            parameters[kvp.Key] = kvp.Value;
                        }
                    }
                }

            }
            catch (JsonException)
            {
                parameters["body"] = "[BODY PARSING FAILED]";
            }
            finally
            {
                httpContext.Request.Body.Position = originalPosition;
            }
        }

        return parameters.Count > 0 ? parameters : null;
    }

    private static Dictionary<string, object>? GetSession(HttpContext? httpContext, string[]? filterKeys = null)
    {
        ISession? session;
        try
        {
            session = httpContext?.Session;
        }
        catch (InvalidOperationException)
        {
            // Session has not been configured for this app
            return null;
        }
        
        if (session is null || !session.IsAvailable)
        {
            return null;
        }

        filterKeys ??= Array.Empty<string>();
        var sessionData = new Dictionary<string, object>();
        foreach (var key in session.Keys)
        {
            var value = session.GetString(key);
            if (value != null)
            {
                sessionData[key] = FilterValue(key, value, filterKeys);
            }
        }

        return sessionData.Count > 0 ? sessionData : null;
    }

    private static CgiData? GetCgiData(HttpContext? httpContext = null)
    {
        if (httpContext == null)
        {
            return null;
        }
        
        var request = httpContext.Request;
        var connection = httpContext.Connection;

        return new CgiData
        {
            HttpCookie = string.Join("; ", request.Cookies.Select(x => $"{x.Key}={x.Value}").ToList()),
            HttpHost = request.Host.Value,
            HttpReferer = request.Headers[HeaderNames.Referer],
            HttpUserAgent = request.Headers[HeaderNames.UserAgent],
            HttpXForwardedFor = request.Headers["X-Forwarded-For"],
            RemoteAddr = connection.RemoteIpAddress?.ToString(),
            RequestMethod = request.Method
        };
    }
}