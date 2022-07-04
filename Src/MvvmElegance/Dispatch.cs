using MvvmElegance.Internal;

namespace MvvmElegance;

public static class Dispatch
{
    private static IDispatcher? _dispatcher;
    private static TaskScheduler? _taskScheduler;
    private static TaskFactory? _taskFactory;

    public static IDispatcher Dispatcher
    {
        get
        {
            if (_dispatcher == null)
            {
                throw new InvalidOperationException("Value cannot be retrieved before method " +
                    $"'{typeof(IBootstrapper).FullName}.{nameof(IBootstrapper.Initialize)}' has been called.");
            }

            return _dispatcher;
        }
    }

    public static TaskScheduler TaskScheduler
    {
        get
        {
            if (_taskScheduler == null)
            {
                throw new InvalidOperationException("Value cannot be retrieved before method " +
                    $"'{typeof(IBootstrapper).FullName}.{nameof(IBootstrapper.Initialize)}' has been called.");
            }

            return _taskScheduler;
        }
    }

    public static TaskFactory TaskFactory
    {
        get
        {
            if (_taskFactory == null)
            {
                throw new InvalidOperationException("Value cannot be retrieved before method " +
                    $"'{typeof(IBootstrapper).FullName}.{nameof(IBootstrapper.Initialize)}' has been called.");
            }

            return _taskFactory;
        }
    }

    public static void Post(Action action)
    {
        if (action is null)
        {
            throw new ArgumentNullException(nameof(action));
        }

        Dispatcher.Post(action);
    }

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

    public static Task PostAsync(Action action)
    {
        if (action is null)
        {
            throw new ArgumentNullException(nameof(action));
        }

        return TaskFactory.StartNew(action, default, TaskFactory.CreationOptions | TaskCreationOptions.DenyChildAttach,
            TaskScheduler);
    }

    public static Task<TResult> PostAsync<TResult>(Func<TResult> func)
    {
        if (func is null)
        {
            throw new ArgumentNullException(nameof(func));
        }

        return TaskFactory.StartNew(func, default, TaskFactory.CreationOptions | TaskCreationOptions.DenyChildAttach,
            TaskScheduler);
    }

    public static Task PostAsync(Func<Task> func)
    {
        if (func is null)
        {
            throw new ArgumentNullException(nameof(func));
        }

        return TaskFactory.StartNew(func, default, TaskFactory.CreationOptions | TaskCreationOptions.DenyChildAttach,
                TaskScheduler)
            .Unwrap();
    }

    public static Task<TResult> PostAsync<TResult>(Func<Task<TResult>> func)
    {
        if (func is null)
        {
            throw new ArgumentNullException(nameof(func));
        }

        return TaskFactory.StartNew(func, default, TaskFactory.CreationOptions | TaskCreationOptions.DenyChildAttach,
                TaskScheduler)
            .Unwrap();
    }

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

    public static Task<TResult> SendOrPostAsync<TResult>(Func<TResult> func)
    {
        if (func is null)
        {
            throw new ArgumentNullException(nameof(func));
        }

        return Dispatcher.IsCurrent ? Task.FromResult(func()) : PostAsync(func);
    }

    public static Task SendOrPostAsync(Func<Task> func)
    {
        if (func is null)
        {
            throw new ArgumentNullException(nameof(func));
        }

        return Dispatcher.IsCurrent ? func() : PostAsync(func);
    }

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
        _taskFactory = new TaskFactory(default, TaskCreationOptions.HideScheduler,
            TaskContinuationOptions.HideScheduler, _taskScheduler);
    }
}
