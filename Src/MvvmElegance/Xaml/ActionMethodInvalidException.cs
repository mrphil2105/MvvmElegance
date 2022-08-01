namespace MvvmElegance.Xaml;

/// <summary>
/// Provides an exception for when the action method has an invalid signature.
/// </summary>
public class ActionMethodInvalidException : Exception
{
    /// <summary>
    /// Initializes a new instance of the <see cref="ActionMethodInvalidException" /> class with the specified message.
    /// </summary>
    /// <param name="message">A message to describe the error.</param>
    public ActionMethodInvalidException(string message) : base(message)
    {
    }
}
