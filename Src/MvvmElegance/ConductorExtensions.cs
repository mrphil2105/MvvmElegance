using System.Collections;

namespace MvvmElegance;

/// <summary>
/// Provides extensions for conductors.
/// </summary>
public static class ConductorExtensions
{
    /// <summary>
    /// Sets the parent of and activates/deactivates the specified item.
    /// </summary>
    /// <param name="conductor">The conductor to set as parent.</param>
    /// <param name="item">The item to set the parent of and activate/deactivate.</param>
    /// <param name="isActive">A boolean indicating whether to activate or deactivate.</param>
    /// <param name="cancellationToken">The cancellation token used to cancel the operation.</param>
    /// <returns>A task representing the asynchronous operation.</returns>
    public static Task SetParentAndSetActiveAsync(
        this IConductor? conductor,
        object? item,
        bool isActive,
        CancellationToken cancellationToken = default
    )
    {
        if (item is IChild child && child.Parent != conductor)
        {
            child.Parent = conductor;
        }

        return isActive
            ? ScreenExtensions.TryActivateAsync(item, cancellationToken)
            : ScreenExtensions.TryDeactivateAsync(item, cancellationToken);
    }

    /// <summary>
    /// Sets the parent of and activates/deactivates the specified items.
    /// </summary>
    /// <param name="conductor">The conductor to set as parent.</param>
    /// <param name="items">A collection with the items to set the parent of and activate/deactivate.</param>
    /// <param name="isActive">A boolean indicating whether to activate or deactivate.</param>
    /// <param name="cancellationToken">The cancellation token used to cancel the operation.</param>
    /// <returns>A task representing the asynchronous operation.</returns>
    /// <exception cref="ArgumentNullException">Value of parameter <paramref name="items" /> is <c>null</c>.</exception>
    public static Task SetParentAndSetActiveAsync(
        this IConductor? conductor,
        IEnumerable items,
        bool isActive,
        CancellationToken cancellationToken = default
    )
    {
        if (items is null)
        {
            throw new ArgumentNullException(nameof(items));
        }

        var tasks = items
            .Cast<object>()
            .Select(i => conductor.SetParentAndSetActiveAsync(i, isActive, cancellationToken))
            .ToList();

        return Task.WhenAll(tasks);
    }

    /// <summary>
    /// Attempts to close and clean the parent of the specified item and optionally dispose it.
    /// </summary>
    /// <param name="conductor">The conductor to compare to the parent.</param>
    /// <param name="item">The item to close, clean and optionally dispose.</param>
    /// <param name="shouldDispose">A boolean indicating whether to dispose the item.</param>
    /// <param name="cancellationToken">The cancellation token used to cancel the operation.</param>
    /// <returns>A task representing the asynchronous operation.</returns>
    public static async Task TryCloseAndCleanUpAsync(
        this IConductor? conductor,
        object? item,
        bool shouldDispose,
        CancellationToken cancellationToken = default
    )
    {
        await ScreenExtensions.TryCloseAsync(item, cancellationToken);

        if (item is IChild child && child.Parent == conductor)
        {
            child.Parent = null;
        }

        if (shouldDispose)
        {
            ScreenExtensions.TryDispose(item);
        }
    }

    /// <summary>
    /// Attempts to close and clean the parent of the specified items and optionally dispose them.
    /// </summary>
    /// <param name="conductor">The conductor to compare to the parents.</param>
    /// <param name="items">A collection with the items to close, clean and optionally dispose.</param>
    /// <param name="shouldDispose">A boolean indicating whether to dispose the items.</param>
    /// <param name="cancellationToken">The cancellation token used to cancel the operation.</param>
    /// <returns>A task representing the asynchronous operation.</returns>
    /// <exception cref="ArgumentNullException">Value of parameter <paramref name="items" /> is <c>null</c>.</exception>
    public static Task TryCloseAndCleanUpAsync(
        this IConductor? conductor,
        IEnumerable items,
        bool shouldDispose,
        CancellationToken cancellationToken = default
    )
    {
        if (items is null)
        {
            throw new ArgumentNullException(nameof(items));
        }

        var tasks = items
            .Cast<object>()
            .Select(i => conductor.TryCloseAndCleanUpAsync(i, shouldDispose, cancellationToken))
            .ToList();

        return Task.WhenAll(tasks);
    }
}
