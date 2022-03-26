namespace MvvmElegance;

/// <summary>
/// Provides event arguments for when a screen has been deactivated.
/// </summary>
public class DeactivatedEventArgs : ScreenStateEventArgs
{
    /// <summary>
    /// Initializes a new instance of the <see cref="DeactivatedEventArgs" /> class with the specified previous screen state.
    /// </summary>
    /// <param name="previousState">The previous state of the screen.</param>
    public DeactivatedEventArgs(ScreenState previousState) : base(previousState)
    {
    }
}
