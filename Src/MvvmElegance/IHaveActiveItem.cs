namespace MvvmElegance;

/// <summary>
/// Represents a model that has an active item.
/// </summary>
/// <typeparam name="T">The type of item.</typeparam>
public interface IHaveActiveItem<T>
{
    /// <summary>
    /// Gets or sets the active item.
    /// </summary>
    T? ActiveItem { get; set; }
}
