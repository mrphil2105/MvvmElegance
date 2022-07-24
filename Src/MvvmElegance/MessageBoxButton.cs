namespace MvvmElegance;

/// <summary>
/// Provides an enumeration of all possible button combinations in a message box.
/// </summary>
public enum MessageBoxButton
{
    /// <summary>
    /// Specifies that an "OK" button should be created.
    /// </summary>
    Ok,
    /// <summary>
    /// Specifies that "OK" and "Cancel" buttons should be created.
    /// </summary>
    OkCancel,
    /// <summary>
    /// Specifies that "Yes" and "No" buttons should be created.
    /// </summary>
    YesNo,
    /// <summary>
    /// Specifies that "Yes", "No" and "Cancel" buttons should be created.
    /// </summary>
    YesNoCancel
}
