namespace MvvmElegance.Internal;

internal abstract class EventSubscriptionStore
{
    public abstract void Add(object subscription);

    public abstract void Remove(object subscription);
}
