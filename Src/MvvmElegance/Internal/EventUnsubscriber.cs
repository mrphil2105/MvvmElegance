namespace MvvmElegance.Internal;

internal class EventUnsubscriber<TEvent> : IDisposable
    where TEvent : IEvent
{
    private readonly EventSubscriptionStore<TEvent> _subscriptionStore;
    private readonly EventSubscription<TEvent> _subscription;

    public EventUnsubscriber(EventSubscriptionStore<TEvent> subscriptionStore,
        EventSubscription<TEvent> subscription)
    {
        _subscriptionStore = subscriptionStore;
        _subscription = subscription;
    }

    private bool _isDisposed;

    public void Dispose()
    {
        if (_isDisposed)
        {
            return;
        }

        _subscriptionStore.Remove(_subscription);
        _isDisposed = true;
    }
}
