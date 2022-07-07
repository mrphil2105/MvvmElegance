namespace MvvmElegance.Internal;

internal class EventSubscription<TEvent>
    where TEvent : IEvent
{
    private readonly Action<TEvent> _action;
    private readonly bool _useDispatcher;

    public EventSubscription(Action<TEvent> action, bool useDispatcher)
    {
        _action = action;
        _useDispatcher = useDispatcher;
    }

    public void Invoke(TEvent e)
    {
        if (_useDispatcher)
        {
            Dispatch.Send(() => _action(e));
        }
        else
        {
            _action(e);
        }
    }
}
