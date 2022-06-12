using System.Collections;

namespace MvvmElegance;

public static class ConductorExtensions
{
    public static Task SetParentAndSetActiveAsync(this IConductor? conductor, object? item, bool isActive,
        CancellationToken cancellationToken = default)
    {
        if (item is IChild child && child.Parent != conductor)
        {
            child.Parent = conductor;
        }

        return isActive
            ? ScreenExtensions.TryActivateAsync(item, cancellationToken)
            : ScreenExtensions.TryDeactivateAsync(item, cancellationToken);
    }

    public static Task SetParentAndSetActiveAsync(this IConductor? conductor, IEnumerable items, bool isActive,
        CancellationToken cancellationToken = default)
    {
        if (items is null)
        {
            throw new ArgumentNullException(nameof(items));
        }

        var tasks = items.Cast<object>()
            .Select(i => conductor.SetParentAndSetActiveAsync(i, isActive, cancellationToken))
            .ToList();

        return Task.WhenAll(tasks);
    }

    public static async Task TryCloseAndCleanUpAsync(this IConductor? conductor, object? item, bool shouldDispose,
        CancellationToken cancellationToken = default)
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

    public static Task TryCloseAndCleanUpAsync(this IConductor? conductor, IEnumerable items, bool shouldDispose,
        CancellationToken cancellationToken = default)
    {
        if (items is null)
        {
            throw new ArgumentNullException(nameof(items));
        }

        var tasks = items.Cast<object>()
            .Select(i => conductor.TryCloseAndCleanUpAsync(i, shouldDispose, cancellationToken))
            .ToList();

        return Task.WhenAll(tasks);
    }
}
