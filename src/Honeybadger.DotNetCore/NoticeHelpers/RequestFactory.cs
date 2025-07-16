using System.Text.Json;
using Honeybadger.Schema;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Http.Extensions; 
using Microsoft.AspNetCore.Routing;
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
            Params = GetParams(httpContext, options?.FilterKeys),
            Session = GetSession(httpContext, options?.FilterKeys),
            Url = FilterUrl(httpContext?.Request.GetEncodedUrl(), options?.FilterKeys),
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
            var queryParams = System.Web.HttpUtility.ParseQueryString(uri.Query);
            var filteredParams = new List<string>();

            foreach (var key in queryParams.AllKeys)
            {
                if (key == null) continue;
                
                var paramValue = queryParams[key] ?? "";
                var value = FilterValue(key, paramValue, filterKeys);
                filteredParams.Add($"{key}={Uri.EscapeDataString(value)}");
            }

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
    
    private static Dictionary<string, object>? GetParams(HttpContext? httpContext, string[]? filterKeys = null)
    {
        return null;
    }

    /// <summary>
    /// todo: I have yet to find a reliable way to read request's body.
    /// HttpContext.Request.Body is a read-forward only stream.
    /// There are ways to enable reading more than once but are not elegant:
    /// - Call HttpContext.Request.EnableBuffering() before reading the request for the first time.
    /// - Read and move stream pointer to 0.
    /// </summary>
    private static async Task<Dictionary<string, object>?> GetParamsAsync(HttpContext? httpContext, string[]? filterKeys = null)
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
            var form =  await httpContext.Request.ReadFormAsync();
            foreach (var formParam in form)
            {
                parameters[formParam.Key] = FilterValue(formParam.Key, formParam.Value, filterKeys);
            }
        }
        
        
        // Get body data if present
        if (httpContext.Request.HasJsonContentType())
        {
            try
            {
                // Assuming body content is JSON, you can deserialize it to a dictionary
                var bodyParams = await httpContext.Request.ReadFromJsonAsync<Dictionary<string, object>>();
                if (bodyParams is not null)
                {
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