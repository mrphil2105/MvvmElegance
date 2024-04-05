namespace MvvmElegance.Xaml;

/// <summary>
/// Provides an exception for when an action target has not been set.
/// </summary>
public class ActionTargetUnsetException : Exception
{
    /// <summary>
    /// Initializes a new instance of the <see cref="ActionTargetUnsetException" /> class with the specified message.
    /// </summary>
    /// <param name="message">A message to describe the error.</param>
    public ActionTargetUnsetException(string message)
        : base(message) { }
}
