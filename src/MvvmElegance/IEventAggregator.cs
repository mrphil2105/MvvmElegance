namespace MvvmElegance;

/// <summary>
/// Represents an event aggregator used for communication between models.
/// </summary>
public interface IEventAggregator
{
    /// <summary>
    /// Adds a subscription to the event of the specified event type.
    /// </summary>
    /// <param name="action">The delegate to execute when the event is raised.</param>
    /// <param name="useDispatcher">A boolean indicating whether the delegate should be executed on the UI thread.</param>
    /// <typeparam name="TEvent">The type of the event to subscribe to.</typeparam>
    /// <returns>A disposable that unsubscribes from the event when disposed.</returns>
    IDisposable Subscribe<TEvent>(Action<TEvent> action, bool useDispatcher = true)
        where TEvent : IEvent;

    /// <summary>
    /// Raises the event of the specified type with the specified data.
    /// </summary>
    /// <param name="e">The event data to raise with the event.</param>
    /// <typeparam name="TEvent">The type of the event being raised.</typeparam>
    void Publish<TEvent>(TEvent e)
        where TEvent : IEvent;
}
