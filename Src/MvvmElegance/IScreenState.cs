namespace MvvmElegance;

/// <summary>
/// Represents the state of a screen and ways to change it.
/// </summary>
public interface IScreenState
{
    /// <summary>
    /// Gets the state of the screen.
    /// </summary>
    ScreenState State { get; }

    /// <summary>
    /// An event that is raised when the screen has been activated.
    /// </summary>
    event EventHandler<ActivatedEventArgs> Activated;

    /// <summary>
    /// An event that is raised when the screen has been deactivated.
    /// </summary>
    event EventHandler<DeactivatedEventArgs> Deactivated;

    /// <summary>
    /// An event that is raised when the screen has been closed.
    /// </summary>
    event EventHandler<ClosedEventArgs> Closed;

    /// <summary>
    /// Activates the screen if it is not already active.
    /// </summary>
    /// <param name="cancellationToken">The cancellation token used to cancel the operation.</param>
    /// <returns>A task representing the asynchronous operation.</returns>
    Task ActivateAsync(CancellationToken cancellationToken = default);

    /// <summary>
    /// Deactivates the screen if it is not already inactive.
    /// </summary>
    /// <param name="cancellationToken">The cancellation token used to cancel the operation.</param>
    /// <returns>A task representing the asynchronous operation.</returns>
    Task DeactivateAsync(CancellationToken cancellationToken = default);

    /// <summary>
    /// Closes the screen if it is not already closed.
    /// </summary>
    /// <param name="cancellationToken">The cancellation token used to cancel the operation.</param>
    /// <returns>A task representing the asynchronous operation.</returns>
    Task CloseAsync(CancellationToken cancellationToken = default);
}
