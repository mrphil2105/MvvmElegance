namespace MvvmElegance.Xaml;

/// <summary>
/// Provides an exception for when the action method has an invalid signature.
/// </summary>
public class ActionGuardInvalidException : Exception
{
    /// <summary>
    /// Initializes a new instance of the <see cref="ActionGuardInvalidException" /> class with the specified message.
    /// </summary>
    /// <param name="message">A message to describe the error.</param>
    public ActionGuardInvalidException(string message) : base(message)
    {
    }
}
