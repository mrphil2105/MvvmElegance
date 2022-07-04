namespace MvvmElegance;

public interface IEventAggregator
{
    IDisposable Subscribe<TEvent>(Action<TEvent> action, bool useDispatcher = true)
        where TEvent : IEvent;

    void Publish<TEvent>(TEvent e)
        where TEvent : IEvent;
}
