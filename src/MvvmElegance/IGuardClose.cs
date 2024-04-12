namespace MvvmElegance;

/// <summary>
/// Represents a model that controls whether it can be closed.
/// </summary>
public interface IGuardClose
{
    /// <summary>
    /// Returns a boolean indicating whether the model can be closed.
    /// </summary>
    /// <param name="cancellationToken">The cancellation token used to cancel the operation.</param>
    /// <returns>A task representing the asynchronous operation, with a boolean indicating whether the model can be closed.</returns>
    Task<bool> CanCloseAsync(CancellationToken cancellationToken = default);
}
