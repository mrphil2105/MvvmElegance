using MvvmElegance.Internal;

namespace MvvmElegance;

/// <summary>
/// Provides helper methods for UI thread dispatching.
/// </summary>
public static class Dispatch
{
    private static IDispatcher? _dispatcher;
    private static TaskScheduler? _taskScheduler;
    private static TaskFactory? _taskFactory;

    /// <summary>
    /// Gets the <see cref="IDispatcher" /> associated with the UI thread.
    /// </summary>
    /// <exception cref="InvalidOperationException">The application <see cref="IBootstrapper" /> has not been initialized.</exception>
    public static IDispatcher Dispatcher
    {
        get
        {
            if (_dispatcher == null)
            {
                throw new InvalidOperationException(
                    "Value cannot be retrieved before method "
                        + $"'{typeof(IBootstrapper).FullName}.{nameof(IBootstrapper.Initialize)}' has been called."
                );
            }

            return _dispatcher;
        }
    }

    /// <summary>
    /// Gets the <see cref="TaskScheduler" /> associated with the UI thread.
    /// </summary>
    /// <exception cref="InvalidOperationException">The application <see cref="IBootstrapper" /> has not been initialized.</exception>
    public static TaskScheduler TaskScheduler
    {
        get
        {
            if (_taskScheduler == null)
            {
                throw new InvalidOperationException(
                    "Value cannot be retrieved before method "
                        + $"'{typeof(IBootstrapper).FullName}.{nameof(IBootstrapper.Initialize)}' has been called."
                );
            }

            return _taskScheduler;
        }
    }

    /// <summary>
    /// Gets the <see cref="TaskFactory" /> associated with the UI thread.
    /// </summary>
    /// <exception cref="InvalidOperationException">The application <see cref="IBootstrapper" /> has not been initialized.</exception>
    public static TaskFactory TaskFactory
    {
        get
        {
            if (_taskFactory == null)
            {
                throw new InvalidOperationException(
                    "Value cannot be retrieved before method "
                        + $"'{typeof(IBootstrapper).FullName}.{nameof(IBootstrapper.Initialize)}' has been called."
                );
            }

            return _taskFactory;
        }
    }

    /// <summary>
    /// Asynchronously dispatches a delegate to the UI thread.
    /// </summary>
    /// <param name="action">The action delegate to dispatch.</param>
    /// <exception cref="ArgumentNullException">Value of parameter <paramref name="action" /> is <c>null</c>.</exception>
    public static void Post(Action action)
    {
        if (action is null)
        {
            throw new ArgumentNullException(nameof(action));
        }

        Dispatcher.Post(action);
    }

    /// <summary>
    /// Synchronously dispatches a delegate to the UI thread or executes in-place if the current thread is the UI thread.
    /// </summary>
    /// <param name="action">The action delegate to dispatch.</param>
    /// <exception cref="ArgumentNullException">Value of parameter <paramref name="action" /> is <c>null</c>.</exception>
    public static void Send(Action action)
    {
        if (action is null)
        {
            throw new ArgumentNullException(nameof(action));
        }

        if (Dispatcher.IsCurrent)
        {
            action();
        }
        else
        {
            Dispatcher.Send(action);
        }
    }

    /// <summary>
    /// Asynchronously dispatches a delegate to the UI thread, as an asynchronous operation.
    /// </summary>
    /// <param name="action">The action delegate to dispatch.</param>
    /// <returns>A task representing the asynchronous operation.</returns>
    /// <exception cref="ArgumentNullException">Value of parameter <paramref name="action" /> is <c>null</c>.</exception>
    public static Task PostAsync(Action action)
    {
        if (action is null)
        {
            throw new ArgumentNullException(nameof(action));
        }

        return TaskFactory.StartNew(
            action,
            default,
            TaskFactory.CreationOptions | TaskCreationOptions.DenyChildAttach,
            TaskScheduler
        );
    }

    /// <summary>
    /// Asynchronously dispatches a delegate with a result to the UI thread, as an asynchronous operation.
    /// </summary>
    /// <param name="func">The func delegate with a result to dispatch.</param>
    /// <typeparam name="TResult">The type of the result produced by the delegate.</typeparam>
    /// <returns>A task representing the asynchronous operation, with a result produced by the delegate.</returns>
    /// <exception cref="ArgumentNullException">Value of parameter <paramref name="func" /> is <c>null</c>.</exception>
    public static Task<TResult> PostAsync<TResult>(Func<TResult> func)
    {
        if (func is null)
        {
            throw new ArgumentNullException(nameof(func));
        }

        return TaskFactory.StartNew(
            func,
            default,
            TaskFactory.CreationOptions | TaskCreationOptions.DenyChildAttach,
            TaskScheduler
        );
    }

    /// <summary>
    /// Asynchronously dispatches an asynchronous delegate to the UI thread, as an asynchronous operation.
    /// </summary>
    /// <param name="func">The asynchronous func delegate to dispatch.</param>
    /// <returns>A task representing the asynchronous operation.</returns>
    /// <exception cref="ArgumentNullException">Value of parameter <paramref name="func" /> is <c>null</c>.</exception>
    public static Task PostAsync(Func<Task> func)
    {
        if (func is null)
        {
            throw new ArgumentNullException(nameof(func));
        }

        return TaskFactory
            .StartNew(func, default, TaskFactory.CreationOptions | TaskCreationOptions.DenyChildAttach, TaskScheduler)
            .Unwrap();
    }

    /// <summary>
    /// Asynchronously dispatches an asynchronous delegate with a result to the UI thread, as an asynchronous operation.
    /// </summary>
    /// <param name="func">The asynchronous func delegate with a result to dispatch.</param>
    /// <typeparam name="TResult">The type of the result produced by the delegate.</typeparam>
    /// <returns>A task representing the asynchronous operation, with a result produced by the delegate.</returns>
    /// <exception cref="ArgumentNullException">Value of parameter <paramref name="func" /> is <c>null</c>.</exception>
    public static Task<TResult> PostAsync<TResult>(Func<Task<TResult>> func)
    {
        if (func is null)
        {
            throw new ArgumentNullException(nameof(func));
        }

        return TaskFactory
            .StartNew(func, default, TaskFactory.CreationOptions | TaskCreationOptions.DenyChildAttach, TaskScheduler)
            .Unwrap();
    }

    /// <summary>
    /// Asynchronously dispatches a delegate to the UI thread or executes in-place if the current thread is the UI thread, as an asynchronous operation.
    /// </summary>
    /// <param name="action">The action delegate to dispatch.</param>
    /// <returns>A task representing the asynchronous operation.</returns>
    /// <exception cref="ArgumentNullException">Value of parameter <paramref name="action" /> is <c>null</c>.</exception>
    public static Task SendOrPostAsync(Action action)
    {
        if (action is null)
        {
            throw new ArgumentNullException(nameof(action));
        }

        if (!Dispatcher.IsCurrent)
        {
            return PostAsync(action);
        }

        action();

        return Task.CompletedTask;
    }

    /// <summary>
    /// Asynchronously dispatches a delegate with a result to the UI thread or executes in-place if the current thread is the UI thread,
    /// as an asynchronous operation.
    /// </summary>
    /// <param name="func">The func delegate with a result to dispatch.</param>
    /// <typeparam name="TResult">The type of the result produced by the delegate.</typeparam>
    /// <returns>A task representing the asynchronous operation, with a result produced by the delegate.</returns>
    /// <exception cref="ArgumentNullException">Value of parameter <paramref name="func" /> is <c>null</c>.</exception>
    public static Task<TResult> SendOrPostAsync<TResult>(Func<TResult> func)
    {
        if (func is null)
        {
            throw new ArgumentNullException(nameof(func));
        }

        return Dispatcher.IsCurrent ? Task.FromResult(func()) : PostAsync(func);
    }

    /// <summary>
    /// Asynchronously dispatches an asynchronous delegate to the UI thread or executes in-place if the current thread is the UI thread,
    /// as an asynchronous operation.
    /// </summary>
    /// <param name="func">The asynchronous func delegate to dispatch.</param>
    /// <returns>A task representing the asynchronous operation.</returns>
    /// <exception cref="ArgumentNullException">Value of parameter <paramref name="func" /> is <c>null</c>.</exception>
    public static Task SendOrPostAsync(Func<Task> func)
    {
        if (func is null)
        {
            throw new ArgumentNullException(nameof(func));
        }

        return Dispatcher.IsCurrent ? func() : PostAsync(func);
    }

    /// <summary>
    /// Asynchronously dispatches an asynchronous delegate with a result to the UI thread or executes in-place if the current thread is the UI thread,
    /// as an asynchronous operation.
    /// </summary>
    /// <param name="func">The asynchronous func delegate with a result to dispatch.</param>
    /// <typeparam name="TResult">The type of the result produced by the delegate.</typeparam>
    /// <returns>A task representing the asynchronous operation, with a result produced by the delegate.</returns>
    /// <exception cref="ArgumentNullException">Value of parameter <paramref name="func" /> is <c>null</c>.</exception>
    public static Task<TResult> SendOrPostAsync<TResult>(Func<Task<TResult>> func)
    {
        if (func is null)
        {
            throw new ArgumentNullException(nameof(func));
        }

        return Dispatcher.IsCurrent ? func() : PostAsync(func);
    }

    internal static void Initialize(IDispatcher dispatcher)
    {
        _dispatcher = dispatcher ?? throw new ArgumentNullException(nameof(dispatcher));
        _taskScheduler = new DispatcherTaskScheduler(dispatcher);
        _taskFactory = new TaskFactory(
            default,
            TaskCreationOptions.HideScheduler,
            TaskContinuationOptions.HideScheduler,
            _taskScheduler
        );
    }
}
