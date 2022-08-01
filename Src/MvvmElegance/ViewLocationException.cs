namespace MvvmElegance;

/// <summary>
/// Provides an exception for when a view type could not be found.
/// </summary>
public class ViewLocationException : Exception
{
    /// <summary>
    /// Initializes a new instance of the <see cref="ViewLocationException" /> class with the specified message and model type.
    /// </summary>
    /// <param name="message">A message to describe the error.</param>
    /// <param name="modelType">The type of model from which a view type could not be found.</param>
    public ViewLocationException(string message, Type modelType) : base(message)
    {
        ModelType = modelType;
    }

    /// <summary>
    /// Gets the type of model from which a view type could not be found.
    /// </summary>
    public Type ModelType { get; }
}
