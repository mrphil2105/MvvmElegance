namespace MvvmElegance;

/// <summary>
/// Represents a parent that has one or more children.
/// </summary>
/// <typeparam name="T">The type of child.</typeparam>
public interface IParent<out T> : IParent
{
    /// <summary>
    /// Returns the children of the parent.
    /// </summary>
    /// <returns>A collection with the children of the parent or <c>null</c> if there are none.</returns>
    new IEnumerable<T>? GetChildren();
}
