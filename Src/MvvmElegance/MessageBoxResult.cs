namespace MvvmElegance;

/// <summary>
/// Provides an enumeration of all possible message box results.
/// </summary>
public enum MessageBoxResult
{
    /// <summary>
    /// Specifies that no message box result is available.
    /// </summary>
    None,
    /// <summary>
    /// Specifies that the user submitted or confirmed an action or some information.
    /// </summary>
    Ok,
    /// <summary>
    /// Specifies that the user cancelled an action.
    /// </summary>
    Cancel,
    /// <summary>
    /// Specifies that the user agreed to perform an action.
    /// </summary>
    Yes,
    /// <summary>
    /// Specifies that the user denied to perform an action.
    /// </summary>
    No
}
