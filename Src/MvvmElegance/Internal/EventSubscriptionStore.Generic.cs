using System.Buffers;
using System.Diagnostics;

namespace MvvmElegance.Internal;

internal class EventSubscriptionStore<TEvent> : EventSubscriptionStore
    where TEvent : IEvent
{
    private readonly List<EventSubscription<TEvent>> _subscriptions;

    public EventSubscriptionStore()
    {
        _subscriptions = new List<EventSubscription<TEvent>>();
    }

    public override void Add(object subscription)
    {
        var typedSubscription = (EventSubscription<TEvent>)subscription;

        lock (_subscriptions)
        {
            _subscriptions.Add(typedSubscription);
        }
    }

    public override void Remove(object subscription)
    {
        var castedSubscription = (EventSubscription<TEvent>)subscription;

        lock (_subscriptions)
        {
            var success = _subscriptions.Remove(castedSubscription);
            Debug.Assert(success);
        }
    }

    public void InvokeAll(TEvent e)
    {
        // Create a copy of the list to invoke the events from, to prevent deadlocks if subscribers call
        // EventAggregator.Subscribe or unsubscribe from another thread while the list lock is held.

        EventSubscription<TEvent>[] subscriptionsCopy;
        int subscriptionCount;

        lock (_subscriptions)
        {
            subscriptionCount = _subscriptions.Count;
            subscriptionsCopy = ArrayPool<EventSubscription<TEvent>>.Shared.Rent(subscriptionCount);
            _subscriptions.CopyTo(subscriptionsCopy);
        }

        try
        {
            for (var i = 0; i < subscriptionCount; i++)
            {
                subscriptionsCopy[i]
                    .Invoke(e);
            }
        }
        finally
        {
            ArrayPool<EventSubscription<TEvent>>.Shared.Return(subscriptionsCopy, true);
        }
    }
}
