namespace MvvmElegance;

/// <summary>
/// Provides an enumeration of all different kinds of message boxes.
/// </summary>
public enum MessageBoxKind
{
    /// <summary>
    /// Specifies that the message box is generic.
    /// </summary>
    None,
    /// <summary>
    /// Specifies that the message box contains an informational message.
    /// </summary>
    Information,
    /// <summary>
    /// Specifies that the message box contains a question.
    /// </summary>
    Question,
    /// <summary>
    /// Specifies that the message box contains a warning message.
    /// </summary>
    Warning,
    /// <summary>
    /// Specifies that the message box contains an error message.
    /// </summary>
    Error
}
