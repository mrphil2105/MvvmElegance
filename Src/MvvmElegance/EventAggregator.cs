using System.Collections.Concurrent;
using MvvmElegance.Internal;

namespace MvvmElegance;

public class EventAggregator : IEventAggregator
{
    private readonly ConcurrentDictionary<Type, EventSubscriptionStore> _subscriptionStores;

    public EventAggregator()
    {
        _subscriptionStores = new ConcurrentDictionary<Type, EventSubscriptionStore>();
    }

    public IDisposable Subscribe<TEvent>(Action<TEvent> action, bool useDispatcher = true)
        where TEvent : IEvent
    {
        if (action is null)
        {
            throw new ArgumentNullException(nameof(action));
        }

        var subscriptionStore = (EventSubscriptionStore<TEvent>)_subscriptionStores.GetOrAdd(typeof(TEvent),
            _ => new EventSubscriptionStore<TEvent>());
        var subscription = new EventSubscription<TEvent>(action, useDispatcher);
        subscriptionStore.Add(subscription);

        return new EventUnsubscriber<TEvent>(subscriptionStore, subscription);
    }

    public void Publish<TEvent>(TEvent e)
        where TEvent : IEvent
    {
        if (e is null)
        {
            throw new ArgumentNullException(nameof(e));
        }

        if (_subscriptionStores.TryGetValue(typeof(TEvent), out var subscriptionStore))
        {
            ((EventSubscriptionStore<TEvent>)subscriptionStore).InvokeAll(e);
        }
    }
}
