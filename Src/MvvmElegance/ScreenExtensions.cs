namespace MvvmElegance;

/// <summary>
/// Provides extensions for screens.
/// </summary>
public static class ScreenExtensions
{
    /// <summary>
    /// Attempts to activate the specified screen if it is not already active.
    /// </summary>
    /// <param name="screen">The screen to activate.</param>
    /// <param name="cancellationToken">The cancellation token used to cancel the operation.</param>
    /// <returns>A task representing the asynchronous operation.</returns>
    public static Task TryActivateAsync(object? screen, CancellationToken cancellationToken = default)
    {
        return screen is IScreenState state ? state.ActivateAsync(cancellationToken) : Task.CompletedTask;
    }

    /// <summary>
    /// Attempts to deactivate the specified screen if it is not already inactive.
    /// </summary>
    /// <param name="screen">The screen to deactivate.</param>
    /// <param name="cancellationToken">The cancellation token used to cancel the operation.</param>
    /// <returns>A task representing the asynchronous operation.</returns>
    public static Task TryDeactivateAsync(object? screen, CancellationToken cancellationToken = default)
    {
        return screen is IScreenState state ? state.DeactivateAsync(cancellationToken) : Task.CompletedTask;
    }

    /// <summary>
    /// Attempts to close the specified screen if it is not already closed.
    /// </summary>
    /// <param name="screen">The screen to close.</param>
    /// <param name="cancellationToken">The cancellation token used to cancel the operation.</param>
    /// <returns>A task representing the asynchronous operation.</returns>
    public static Task TryCloseAsync(object? screen, CancellationToken cancellationToken = default)
    {
        return screen is IScreenState state ? state.CloseAsync(cancellationToken) : Task.CompletedTask;
    }

    /// <summary>
    /// Attempts to dispose the specified screen.
    /// </summary>
    /// <param name="screen">The screen to dispose.</param>
    public static void TryDispose(object? screen)
    {
        if (screen is IDisposable disposable)
        {
            disposable.Dispose();
        }
    }
}
