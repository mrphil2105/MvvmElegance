namespace MvvmElegance;

/// <summary>
/// Represents a conductor that has one or more children.
/// </summary>
public interface IConductor : IParent
{
    /// <summary>
    /// Activates the specified item.
    /// </summary>
    /// <param name="item">The item to activate.</param>
    /// <param name="cancellationToken">The cancellation token used to cancel the operation.</param>
    /// <returns>A task representing the asynchronous operation.</returns>
    Task ActivateItemAsync(object? item, CancellationToken cancellationToken = default);

    /// <summary>
    /// Deactivates the specified item.
    /// </summary>
    /// <param name="item">The item to deactivate.</param>
    /// <param name="cancellationToken">The cancellation token used to cancel the operation.</param>
    /// <returns>A task representing the asynchronous operation.</returns>
    Task DeactivateItemAsync(object? item, CancellationToken cancellationToken = default);

    /// <summary>
    /// Closes the specified item.
    /// </summary>
    /// <param name="item">The item to close.</param>
    /// <param name="cancellationToken">The cancellation token used to cancel the operation.</param>
    /// <returns>A task representing the asynchronous operation.</returns>
    Task CloseItemAsync(object? item, CancellationToken cancellationToken = default);
}
