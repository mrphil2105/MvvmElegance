namespace MvvmElegance.Xaml;

/// <summary>
/// Provides an enumeration of all behaviors that can be performed if the target is <c>null</c> or the action method does not exist.
/// </summary>
public enum ActionUnavailableBehavior
{
    /// <summary>
    /// Specifies that default behavior should be used. It depends on whether the behavior applies to an action method or a target.
    /// </summary>
    Default,

    /// <summary>
    /// Specifies that an exception should be thrown.
    /// </summary>
    Throw,

    /// <summary>
    /// Specifies that the control should be enabled and do nothing.
    /// </summary>
    Enable,

    /// <summary>
    /// Specifies that the control should be disabled.
    /// </summary>
    Disable
}
