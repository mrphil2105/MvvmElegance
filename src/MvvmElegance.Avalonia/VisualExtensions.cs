using Avalonia.VisualTree;

namespace MvvmElegance;

/// <summary>
/// Provides extensions for visuals.
/// </summary>
public static class VisualExtensions
{
    /// <summary>
    /// Gets the visual children of the specified type of the specified visual.
    /// </summary>
    /// <param name="visual">The visual to get the children of.</param>
    /// <typeparam name="TChild">The type of child to get.</typeparam>
    /// <returns>A collection containing the visual children of the specified visual.</returns>
    /// <exception cref="ArgumentNullException">Value of parameter <paramref name="visual" /> is <c>null</c>.</exception>
    public static IEnumerable<TChild> GetVisualChildren<TChild>(this IVisual visual)
        where TChild : IVisual
    {
        if (visual is null)
        {
            throw new ArgumentNullException(nameof(visual));
        }

        foreach (var child in visual.GetVisualChildren())
        {
            if (child is TChild typedChild)
            {
                yield return typedChild;
            }

            foreach (var childOfChild in child.GetVisualChildren<TChild>())
            {
                yield return childOfChild;
            }
        }
    }

    /// <summary>
    /// Attempts to get the parent window of the specified visual.
    /// </summary>
    /// <param name="visual">The visual to get the parent window of.</param>
    /// <returns>The parent window of the visual or <c>null</c> if not found.</returns>
    /// <exception cref="InvalidOperationException">The visual root is not of type <see cref="Window" />.</exception>
    public static Window? GetWindow(this IVisual visual)
    {
        var visualRoot = visual.GetVisualRoot();

        if (visualRoot == null)
        {
            return null;
        }

        if (visualRoot is not Window window)
        {
            throw new InvalidOperationException($"The visual root must be of type '{typeof(Window).FullName}'.");
        }

        return window;
    }
}
