namespace MvvmElegance.Xaml;

/// <summary>
/// Provides an exception for when the action method could not be found on the target.
/// </summary>
public class ActionMethodNotFoundException : Exception
{
    /// <summary>
    /// Initializes a new instance of the <see cref="ActionMethodNotFoundException" /> class with the specified message.
    /// </summary>
    /// <param name="message">A message to describe the error.</param>
    public ActionMethodNotFoundException(string message)
        : base(message) { }
}
