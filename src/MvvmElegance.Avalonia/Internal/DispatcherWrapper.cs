using Avalonia.Threading;

namespace MvvmElegance.Internal;

internal class DispatcherWrapper : IDispatcher
{
    private readonly Dispatcher _dispatcher;

    public DispatcherWrapper(Dispatcher dispatcher)
    {
        _dispatcher = dispatcher ?? throw new ArgumentNullException(nameof(dispatcher));
    }

    public bool IsCurrent => _dispatcher.CheckAccess();

    public void Post(Action action)
    {
        _dispatcher.Post(action, DispatcherPriority.Send);
    }

    public void Send(Action action)
    {
        if (IsCurrent)
        {
            action();
        }
        else
        {
            _dispatcher.InvokeAsync(action, DispatcherPriority.Send).Wait();
        }
    }
}
