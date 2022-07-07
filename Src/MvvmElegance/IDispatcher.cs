namespace MvvmElegance;

/// <summary>
/// Represents a dispatcher used for thread dispatching.
/// </summary>
public interface IDispatcher
{
    /// <summary>
    /// Gets a boolean indicating whether the current thread is associated with the <see cref="IDispatcher" />.
    /// </summary>
    bool IsCurrent { get; }

    /// <summary>
    /// Asynchronously dispatches a delegate to the thread associated with the <see cref="IDispatcher" />.
    /// </summary>
    /// <param name="action">The action delegate to dispatch.</param>
    void Post(Action action);

    /// <summary>
    /// Synchronously dispatches a delegate to the thread associated with the <see cref="IDispatcher" />.
    /// </summary>
    /// <param name="action">The action delegate to dispatch.</param>
    void Send(Action action);
}
