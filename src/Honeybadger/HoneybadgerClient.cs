using System.Text;
using System.Text.Json;
using System.Text.Json.Serialization;
using Honeybadger.Schema;
using Microsoft.Extensions.Options;

namespace Honeybadger;

public class HoneybadgerClient : IHoneybadgerClient, IDisposable
{
    public HoneybadgerOptions Options { get; private set; } = null!;
    
    private HttpClient? _httpClient;

    private readonly ThreadLocal<Dictionary<string, object>> _context;

    private readonly ThreadLocal<List<Trail>> _breadcrumbs;
    
    private readonly NoticeFactory _noticeFactory;

    public HoneybadgerClient(
        IOptions<HoneybadgerOptions> options,
        NoticeFactory? noticeFactory = null
    )
    {
        _noticeFactory = noticeFactory ?? new NoticeFactory();
        _context = new ThreadLocal<Dictionary<string, object>>(() => new Dictionary<string, object>());
        _breadcrumbs = new ThreadLocal<List<Trail>>(() => new List<Trail>());
        Configure(options.Value);
    }

    public void Notify(string message)
    {
        Notify(message, new Dictionary<string, object>());
    }

    public void Notify(string message, Dictionary<string, object> context)
    {
        var notice = _noticeFactory.Make(this, message, GetContext(context));
        Notify(notice);
    }

    public void Notify(Exception error)
    {
        Notify(error, new Dictionary<string, object>());
    }

    public void Notify(Exception error, Dictionary<string, object> context)
    {
        var notice = _noticeFactory.Make(this, error, GetContext(context));
        Notify(notice);
    }

    public void Notify(Notice notice)
    {
        _ = Task.Run(async () => await NotifyAsync(notice)).ConfigureAwait(false);
    }

    public Task NotifyAsync(string message)
    {
        return NotifyAsync(message, new Dictionary<string, object>());
    }

    public async Task NotifyAsync(string message, Dictionary<string, object> context)
    {
        var notice = _noticeFactory.Make(this, message, GetContext(context));
        await NotifyAsync(notice);
    }

    public Task NotifyAsync(Exception error)
    {
        return NotifyAsync(error, new Dictionary<string, object>());
    }

    public async Task NotifyAsync(Exception error, Dictionary<string, object> context)
    {
        var notice = _noticeFactory.Make(this, error, GetContext(context));
        await NotifyAsync(notice);
    }
    
    public async Task NotifyAsync(Notice notice)
    {
        await Send(notice);
    }

    public void AddContext(Dictionary<string, object> context)
    {
        if (_context.Value == null)
        {
            return;
        }
        
        foreach (var entry in context)
        {
            _context.Value[entry.Key] = entry.Value;
        }
    }

    public void ResetContext()
    {
        _context.Value?.Clear();
    }

    public void AddBreadcrumb(string message, string? category = null, Dictionary<string, object?>? options = null)
    {
        if (_breadcrumbs.Value == null || !Options.ShouldReport() || !Options.BreadcrumbsEnabled)
        {
            // fixme: for debugging purposes (see #3 - CI is randomly failing) 
            // Console.WriteLine($"not adding breadcrumb: {message}");
            return;
        }

        var trail = new Trail(message);
        if (category != null)
        {
            trail.Category = category;
        }

        if (options != null)
        {
            trail.Metadata = options;
        }
        
        _breadcrumbs.Value.Add(trail);

        if (_breadcrumbs.Value.Count > Options.MaxBreadcrumbs)
        {
            _breadcrumbs.Value.RemoveRange(0, _breadcrumbs.Value.Count - Options.MaxBreadcrumbs);
        }
    }

    public void ResetBreadcrumbs()
    {
        _breadcrumbs.Value?.Clear();
    }

    public void Configure(HoneybadgerOptions options)
    {
        Options = options;
        SetProjectRoot();
        GetClient(true);
    }
    
    public void Dispose()
    {
        _context.Dispose();
        _breadcrumbs.Dispose();
    }

    public Dictionary<string, object> GetContext(Dictionary<string, object>? context = null)
    {
        if (context == null)
        {
            return _context.Value ?? new Dictionary<string, object>();
        }

        if (_context.Value == null)
        {
            return context;
        }

        foreach (var entry in context)
        {
            _context.Value[entry.Key] = entry.Value;
        }

        return _context.Value;
    }

    Trail[]? IHoneybadgerClient.GetBreadcrumbs()
    {
        return Options.BreadcrumbsEnabled ? _breadcrumbs.Value?.ToArray() : null;
    }

    private async Task Send(Notice notice)
    {
        if (!Options.ShouldReport())
        {
            return;
        }
        
        // Console.WriteLine("Ready to send report to Honeybadger");
        var request = new HttpRequestMessage(HttpMethod.Post, "v1/notices");
        var json = JsonSerializer.Serialize(notice, new JsonSerializerOptions
        {
            DefaultIgnoreCondition = JsonIgnoreCondition.WhenWritingNull
        });
        request.Content = new StringContent(json, Encoding.UTF8, "application/json");
        try
        {
            var result = await GetClient().SendAsync(request);
            if (!result.IsSuccessStatusCode)
            {
                // var content = await result.Content.ReadAsStringAsync();
                // Console.WriteLine("Could not send report to Honeybadger | HTTP[{0}]: {1}", result.StatusCode, content);
            }
            else
            {
                // Console.WriteLine("Report sent to Honeybadger");
            }
        }
        catch (Exception)
        {
            // no op
            // await Console.Error.WriteLineAsync(ex.Message);
        }
    }

    private HttpClient GetClient(bool forceNew = false)
    {
        if (_httpClient is not null && !forceNew)
        {
            return _httpClient;
        }
        
        if (Options.HttpClient is not null)
        {
            _httpClient = Options.HttpClient;
        }
        else
        {
            var client = new HttpClient();
            client.BaseAddress = Options.Endpoint;
            client.DefaultRequestHeaders.Add("X-API-Key", Options.ApiKey);
            _httpClient = client;
        }
        
        return _httpClient;
    }
    
    /// <summary>
    /// Attempts to set the project root directory based on the base directory from AppDomain.
    /// Note: This is a best-effort approach and may not work in all scenarios.
    ///       We also check if the directory exists, which could throw an exception if
    ///       the process does not have access to the directory. In that case, we do not set ProjectRoot.
    /// </summary>
    private void SetProjectRoot()
    {
        if (Options.ProjectRoot is not null)
        {
            return;
        }

        try
        {
            // Get the project root directory, excluding the bin directory
            // Note: If the project is part of a solution, it's better to use the solution root
            var projectRootDir = Path.GetFullPath("../../../", AppDomain.CurrentDomain.BaseDirectory);
            if (!Directory.Exists(projectRootDir))
            {
                return;
            }
        
            // Now let's see if we can find the solution file
            // by going up the directory tree, one level at a time
            // We go a maximum of 5 levels up
            const int maxLevels = 5;
            for (var i = 0; i < maxLevels; i++)
            {
                // .sln and .slnx are the solution file extensions
                if (Directory.GetFiles(projectRootDir, "*.sln", SearchOption.TopDirectoryOnly).Length != 0 ||
                    Directory.GetFiles(projectRootDir, "*.slnx", SearchOption.TopDirectoryOnly).Length != 0)
                {
                    break;
                }
                projectRootDir = Directory.GetParent(projectRootDir)?.FullName ?? projectRootDir;
            }
        
            Options.ProjectRoot = projectRootDir;
        }
        catch (Exception)
        {
            // no op
        }
    }
}