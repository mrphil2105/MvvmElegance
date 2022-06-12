using System.Collections;

namespace MvvmElegance;

/// <summary>
/// Provides the base class for conductors.
/// </summary>
/// <typeparam name="T">The type of item to conduct.</typeparam>
public abstract class ConductorBase<T> : Screen, IConductor, IParent<T>
    where T : class
{
    private bool _disposeChildren;

    /// <summary>
    /// Initializes a new instance of the <see cref="ConductorBase{T}" /> class.
    /// </summary>
    protected ConductorBase()
    {
        _disposeChildren = true;
    }

    /// <summary>
    /// Gets or sets a boolean indicating whether the children should be disposed.
    /// </summary>
    protected virtual bool DisposeChildren
    {
        get => _disposeChildren;
        set => _disposeChildren = value;
    }

    /// <summary>
    /// Activates the specified item.
    /// </summary>
    /// <param name="item">The item to activate.</param>
    /// <param name="cancellationToken">The cancellation token used to cancel the operation.</param>
    /// <returns>A task representing the asynchronous operation.</returns>
    public abstract Task ActivateItemAsync(T? item, CancellationToken cancellationToken = default);

    /// <summary>
    /// Deactivates the specified item.
    /// </summary>
    /// <param name="item">The item to deactivate.</param>
    /// <param name="cancellationToken">The cancellation token used to cancel the operation.</param>
    /// <returns>A task representing the asynchronous operation.</returns>
    public abstract Task DeactivateItemAsync(T? item, CancellationToken cancellationToken = default);

    /// <summary>
    /// Closes the specified item.
    /// </summary>
    /// <param name="item">The item to close.</param>
    /// <param name="cancellationToken">The cancellation token used to cancel the operation.</param>
    /// <returns>A task representing the asynchronous operation.</returns>
    public abstract Task CloseItemAsync(T? item, CancellationToken cancellationToken = default);

    /// <summary>
    /// Returns the children in a collection.
    /// </summary>
    /// <returns>A collection containing the children.</returns>
    public abstract IEnumerable<T> GetChildren();

    /// <summary>
    /// Ensures the specified item is ready for activation/deactivation by setting the parent.
    /// </summary>
    /// <param name="item">The item to ensure.</param>
    protected virtual void EnsureItem(T? item)
    {
        if (item is IChild child && child.Parent != this)
        {
            child.Parent = this;
        }
    }

    /// <summary>
    /// Returns a boolean indicating whether the specified item can be closed.
    /// </summary>
    /// <param name="item">The item to check.</param>
    /// <param name="cancellationToken">The cancellation token used to cancel the operation.</param>
    /// <returns>A task representing the asynchronous operation, with a boolean indicating whether the item can be closed.</returns>
    protected virtual Task<bool> CanCloseItemAsync(T? item, CancellationToken cancellationToken = default)
    {
        return item is IGuardClose guardClose ? guardClose.CanCloseAsync(cancellationToken) : Task.FromResult(true);
    }

    /// <summary>
    /// Returns a boolean indicating whether all the specified items can be closed.
    /// </summary>
    /// <param name="items">The collection with the items to check.</param>
    /// <param name="cancellationToken">The cancellation token used to cancel the operation.</param>
    /// <returns>A task representing the asynchronous operation, with a boolean indicating whether the items can be closed.</returns>
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
