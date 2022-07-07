using System.Collections;

namespace MvvmElegance;

/// <summary>
/// Represents a parent that has one or more children.
/// </summary>
public interface IParent
{
    /// <summary>
    /// Returns the children of the parent.
    /// </summary>
    /// <returns>A collection with the children of the parent or <c>null</c> if there are none.</returns>
    IEnumerable? GetChildren();
}
