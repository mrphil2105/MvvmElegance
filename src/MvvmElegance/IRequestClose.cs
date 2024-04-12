namespace MvvmElegance;

/// <summary>
/// Represents a model that can be requested to close.
/// </summary>
public interface IRequestClose
{
    /// <summary>
    /// Requests that the model should be closed.
    /// </summary>
    /// <param name="dialogResult">The dialog result to set if applicable.</param>
    /// <param name="cancellationToken">The cancellation token used to cancel the operation.</param>
    /// <returns>A task representing the asynchronous operation, with a boolean indicating whether the model has been closed.</returns>
    Task<bool> TryCloseAsync(bool? dialogResult = null, CancellationToken cancellationToken = default);
}
