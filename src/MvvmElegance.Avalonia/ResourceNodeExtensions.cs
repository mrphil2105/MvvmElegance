namespace MvvmElegance;

/// <summary>
/// Provides extensions for resource nodes.
/// </summary>
public static class ResourceNodeExtensions
{
    /// <summary>
    /// Finds the resource of the specified type by searching up the logical tree and then global styles.
    /// </summary>
    /// <param name="control">The control to search from.</param>
    /// <param name="key">The resource key.</param>
    /// <typeparam name="TResource">The type of resource.</typeparam>
    /// <returns>The found resource of the specified resource type, or <see cref="AvaloniaProperty.UnsetValue" /> if not found.</returns>
    /// <exception cref="ArgumentNullException">Value of parameter <paramref name="control" /> is <c>null</c>.</exception>
    public static TResource? FindResource<TResource>(this IResourceHost control, object key)
    {
        if (control is null)
        {
            throw new ArgumentNullException(nameof(control));
        }

        return (TResource?)control.FindResource(key);
    }

    /// <summary>
    /// Attempts to find the resource of the specified type by searching up the logical tree and then global styles.
    /// </summary>
    /// <param name="control">The control to search from.</param>
    /// <param name="key">The resource key.</param>
    /// <param name="value">The found resource of the specified resource type, or <see cref="AvaloniaProperty.UnsetValue" /> if not found.</param>
    /// <typeparam name="TResource">The type of resource.</typeparam>
    /// <returns>A boolean indicating whether the resource could be found.</returns>
    /// <exception cref="ArgumentNullException">Value of parameter <paramref name="control" /> is <c>null</c>.</exception>
    public static bool TryFindResource<TResource>(this IResourceHost control, object key, out TResource? value)
    {
        if (control is null)
        {
            throw new ArgumentNullException(nameof(control));
        }

        var success = control.TryFindResource(key, out var resource);
        value = (TResource?)resource;

        return success;
    }
}
