namespace Honeybadger.DotNetCore.Utils;

/// <summary>
/// Provides utilities for safely calling async methods from synchronous contexts.
/// These helpers prevent deadlocks and ensure proper exception propagation by using
/// a dedicated TaskScheduler.Default to avoid capturing the synchronization context.
/// </summary>
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