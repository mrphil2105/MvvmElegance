namespace MvvmElegance;

/// <summary>
/// Represents a type locator for views.
/// </summary>
public interface IViewTypeLocator
{
    /// <summary>
    /// Returns the view type associated with the specified model type.
    /// </summary>
    /// <param name="modelType">The type of the model.</param>
    /// <returns>A type associated with the model type.</returns>
    Type Locate(Type modelType);
}
