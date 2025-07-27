namespace Honeybadger.DotNetCore.Utils;

public static class AsyncHelper
{
    private static readonly TaskFactory TaskFactory = new
        TaskFactory(CancellationToken.None,
            TaskCreationOptions.None,
            TaskContinuationOptions.None,
            TaskScheduler.Default);

    public static TResult RunSync<TResult>(Func<Task<TResult>> func, CancellationToken cancellationToken = default)
        => TaskFactory
            .StartNew(func, cancellationToken)
            .Unwrap()
            .GetAwaiter()
            .GetResult();

    public static void RunSync(Func<Task> func, CancellationToken cancellationToken = default)
        => TaskFactory
            .StartNew(func, cancellationToken)
            .Unwrap()
            .GetAwaiter()
            .GetResult();
}