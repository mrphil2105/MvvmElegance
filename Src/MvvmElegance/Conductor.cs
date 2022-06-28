namespace MvvmElegance;

public partial class Conductor<T> : ConductorBaseWithActiveItem<T>
    where T : class
{
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

    public override Task DeactivateItemAsync(T? item, CancellationToken cancellationToken = default)
    {
        if (item != null && EqualityComparer<T?>.Default.Equals(item, ActiveItem))
        {
            return ScreenExtensions.TryDeactivateAsync(ActiveItem, cancellationToken);
        }

        return Task.CompletedTask;
    }

    public override async Task CloseItemAsync(T? item, CancellationToken cancellationToken = default)
    {
        if (item != null && EqualityComparer<T?>.Default.Equals(item, ActiveItem) &&
            await CanCloseItemAsync(item, cancellationToken))
        {
            await ChangeActiveItemAsync(null, true, cancellationToken);
        }
    }

    public override IEnumerable<T> GetChildren()
    {
        return ActiveItem != null ? new[] { ActiveItem } : Array.Empty<T>();
    }

    public override Task<bool> CanCloseAsync(CancellationToken cancellationToken = default)
    {
        return CanCloseItemAsync(ActiveItem, cancellationToken);
    }
}
