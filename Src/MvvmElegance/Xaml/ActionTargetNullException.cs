namespace MvvmElegance.Xaml;

/// <summary>
/// Provides an exception for when an action target is null.
/// </summary>
public class ActionTargetNullException : Exception
{
    /// <summary>
    /// Initializes a new instance of the <see cref="ActionTargetNullException" /> class with the specified message.
    /// </summary>
    /// <param name="message">A message to describe the error.</param>
    public ActionTargetNullException(string message) : base(message)
    {
    }
}
