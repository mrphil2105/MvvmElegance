namespace MvvmElegance;

/// <summary>
/// Provides event arguments for when a screen has been closed.
/// </summary>
public class ClosedEventArgs : ScreenStateEventArgs
{
    /// <summary>
    /// Initializes a new instance of the <see cref="ClosedEventArgs" /> class with the specified previous screen state.
    /// </summary>
    /// <param name="previousState">The previous state of the screen.</param>
    public ClosedEventArgs(ScreenState previousState) : base(previousState)
    {
    }
}
