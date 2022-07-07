namespace MvvmElegance;

/// <summary>
/// Provides a conductor with a single item.
/// </summary>
/// <typeparam name="T">The type of item to conduct.</typeparam>
public partial class Conductor<T> : ConductorBaseWithActiveItem<T>
    where T : class
{
    /// <summary>
    /// Replaces the active item with the specified item and activates it, if the active item can be closed.
    /// </summary>
    /// <param name="item">The item to activate and set as active item.</param>
    /// <param name="cancellationToken">The cancellation token used to cancel the operation.</param>
    /// <returns>A task representing the asynchronous operation.</returns>
    public override async Task ActivateItemAsync(T? item, CancellationToken cancellationToken = default)
    {
        if (item != null && EqualityComparer<T?>.Default.Equals(item, ActiveItem))
        {
            if (IsActive)
            {
                await ScreenExtensions.TryActivateAsync(ActiveItem, cancellationToken);
            }
        }
        else if (await CanCloseItemAsync(ActiveItem, cancellationToken))
        {
            await ChangeActiveItemAsync(item, true, cancellationToken);
        }
    }

    /// <summary>
    /// Deactivates the specified item if it is the active item.
    /// </summary>
    /// <param name="item">The item to deactivate.</param>
    /// <param name="cancellationToken">The cancellation token used to cancel the operation.</param>
    /// <returns>A task representing the asynchronous operation.</returns>
    public override Task DeactivateItemAsync(T? item, CancellationToken cancellationToken = default)
    {
        if (item != null && EqualityComparer<T?>.Default.Equals(item, ActiveItem))
        {
            return ScreenExtensions.TryDeactivateAsync(ActiveItem, cancellationToken);
        }

        return Task.CompletedTask;
    }

    /// <summary>
    /// Closes the specified item if it can be closed and if it is the active item.
    /// </summary>
    /// <param name="item">The item to close.</param>
    /// <param name="cancellationToken">The cancellation token used to cancel the operation.</param>
    /// <returns>A task representing the asynchronous operation.</returns>
    public override async Task CloseItemAsync(T? item, CancellationToken cancellationToken = default)
    {
        if (item != null && EqualityComparer<T?>.Default.Equals(item, ActiveItem) &&
            await CanCloseItemAsync(item, cancellationToken))
        {
            await ChangeActiveItemAsync(null, true, cancellationToken);
        }
    }

    /// <summary>
    /// Returns the active item in a collection.
    /// </summary>
    /// <returns>A collection containing the active item.</returns>
    public override IEnumerable<T> GetChildren()
    {
        return ActiveItem != null ? new[] { ActiveItem } : Array.Empty<T>();
    }

    /// <inheritdoc />
    public override Task<bool> CanCloseAsync(CancellationToken cancellationToken = default)
    {
        return CanCloseItemAsync(ActiveItem, cancellationToken);
    }
}
