namespace MvvmElegance;

/// <summary>
/// Provides the base class for conductors that have an active item.
/// </summary>
/// <typeparam name="T">The type of item to conduct.</typeparam>
public abstract class ConductorBaseWithActiveItem<T> : ConductorBase<T>, IHaveActiveItem<T>
    where T : class
{
    private T? _activeItem;

    /// <inheritdoc />
    public T? ActiveItem
    {
        get => _activeItem;
        set => ActivateItemAsync(value);
    }

    /// <summary>
    /// Changes the active item and optionally closes the previous active item.
    /// </summary>
    /// <param name="newItem">The item to change the active item to.</param>
    /// <param name="closePrevious">A boolean indicating whether to close the previous active item.</param>
    /// <param name="cancellationToken">The cancellation token used to cancel the operation.</param>
    protected virtual async Task ChangeActiveItemAsync(T? newItem, bool closePrevious,
        CancellationToken cancellationToken = default)
    {
        await ScreenExtensions.TryDeactivateAsync(ActiveItem, cancellationToken);

        if (closePrevious)
        {
            await this.TryCloseAndCleanUpAsync(ActiveItem, DisposeChildren, cancellationToken);
        }

        if (newItem != null)
        {
            EnsureItem(newItem);

            if (IsActive)
            {
                await ScreenExtensions.TryActivateAsync(newItem, cancellationToken);
            }
            else
            {
                await ScreenExtensions.TryDeactivateAsync(newItem, cancellationToken);
            }
        }

        // We do not set 'ActiveItem' here as it itself calls 'ChangeActiveItemAsync' in concrete conductors.
        _activeItem = newItem;
        RaisePropertyChanged(nameof(ActiveItem));
    }

    /// <inheritdoc />
    protected override Task OnActivateAsync(CancellationToken cancellationToken = default)
    {
        return ScreenExtensions.TryActivateAsync(ActiveItem, cancellationToken);
    }

    /// <inheritdoc />
    protected override Task OnDeactivateAsync(CancellationToken cancellationToken = default)
    {
        return ScreenExtensions.TryDeactivateAsync(ActiveItem, cancellationToken);
    }

    /// <inheritdoc />
    protected override Task OnCloseAsync(CancellationToken cancellationToken = default)
    {
        return this.TryCloseAndCleanUpAsync(ActiveItem, DisposeChildren, cancellationToken);
    }
}
