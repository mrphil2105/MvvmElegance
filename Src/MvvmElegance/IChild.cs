namespace MvvmElegance;

/// <summary>
/// Represents a child that has a parent.
/// </summary>
public interface IChild
{
    /// <summary>
    /// Gets or sets the parent.
    /// </summary>
    object? Parent { get; set; }
}
