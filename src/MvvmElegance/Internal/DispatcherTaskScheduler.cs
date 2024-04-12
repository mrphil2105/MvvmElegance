namespace MvvmElegance.Internal;

internal class DispatcherTaskScheduler : TaskScheduler
{
    private readonly IDispatcher _dispatcher;

    public DispatcherTaskScheduler(IDispatcher dispatcher)
    {
        _dispatcher = dispatcher ?? throw new ArgumentNullException(nameof(dispatcher));
    }

    public override int MaximumConcurrencyLevel => 1;

    protected override void QueueTask(Task task)
    {
        _dispatcher.Post(() => TryExecuteTask(task));
    }

    protected override bool TryExecuteTaskInline(Task task, bool wasPreviouslyQueued)
    {
        return _dispatcher.IsCurrent && TryExecuteTask(task);
    }

    protected override IEnumerable<Task>? GetScheduledTasks()
    {
        return null;
    }
}
