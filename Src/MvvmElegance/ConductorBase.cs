using System.Collections;

namespace MvvmElegance;

public abstract class ConductorBase<T> : Screen, IConductor, IParent<T>
    where T : class
{
    private bool _disposeChildren;

    protected ConductorBase()
    {
        _disposeChildren = true;
    }

    protected virtual bool DisposeChildren
    {
        get => _disposeChildren;
        set => _disposeChildren = value;
    }

    public abstract Task ActivateItemAsync(T? item, CancellationToken cancellationToken = default);

    public abstract Task DeactivateItemAsync(T? item, CancellationToken cancellationToken = default);

    public abstract Task CloseItemAsync(T? item, CancellationToken cancellationToken = default);

    public abstract IEnumerable<T> GetChildren();

    protected virtual void EnsureItem(T? item)
    {
        if (item is IChild child && child.Parent != this)
        {
            child.Parent = this;
        }
    }

    protected virtual Task<bool> CanCloseItemAsync(T? item, CancellationToken cancellationToken = default)
    {
        return item is IGuardClose guardClose ? guardClose.CanCloseAsync(cancellationToken) : Task.FromResult(true);
    }

    protected virtual async Task<bool> CanCloseAllItemsAsync(IEnumerable<T?>? items,
        CancellationToken cancellationToken = default)
    {
        if (items == null)
        {
            return true;
        }

        foreach (var item in items)
        {
            // We must call each 'CanCloseItemAsync' in sequence to prevent multiple potential dialog popups.
            if (!await CanCloseItemAsync(item, cancellationToken))
            {
                return false;
            }
        }

        return true;
    }

    Task IConductor.ActivateItemAsync(object? item, CancellationToken cancellationToken)
    {
        return ActivateItemAsync((T?)item, cancellationToken);
    }

    Task IConductor.DeactivateItemAsync(object? item, CancellationToken cancellationToken)
    {
        return DeactivateItemAsync((T?)item, cancellationToken);
    }

    Task IConductor.CloseItemAsync(object? item, CancellationToken cancellationToken)
    {
        return CloseItemAsync((T?)item, cancellationToken);
    }

    IEnumerable IParent.GetChildren()
    {
        return GetChildren();
    }
}
