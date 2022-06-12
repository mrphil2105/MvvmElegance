namespace MvvmElegance;

public abstract class ConductorBaseWithActiveItem<T> : ConductorBase<T>, IHaveActiveItem<T>
    where T : class
{
    private T? _activeItem;

    public T? ActiveItem
    {
        get => _activeItem;
        set => ActivateItemAsync(value);
    }

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

    protected override Task OnActivateAsync(CancellationToken cancellationToken = default)
    {
        return ScreenExtensions.TryActivateAsync(ActiveItem, cancellationToken);
    }

    protected override Task OnDeactivateAsync(CancellationToken cancellationToken = default)
    {
        return ScreenExtensions.TryDeactivateAsync(ActiveItem, cancellationToken);
    }

    protected override Task OnCloseAsync(CancellationToken cancellationToken = default)
    {
        return this.TryCloseAndCleanUpAsync(ActiveItem, DisposeChildren, cancellationToken);
    }
}
