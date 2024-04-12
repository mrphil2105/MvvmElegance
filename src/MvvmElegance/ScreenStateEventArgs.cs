namespace MvvmElegance;

/// <summary>
/// Provides event arguments for when the state of a screen has changed.
/// </summary>
public abstract class ScreenStateEventArgs : EventArgs
{
    /// <summary>
    /// Initializes a new instance of the <see cref="ScreenStateEventArgs" /> class with the specified previous screen state.
    /// </summary>
    /// <param name="previousState">The previous state of the screen.</param>
    protected ScreenStateEventArgs(ScreenState previousState)
    {
        PreviousState = previousState;
    }

    /// <summary>
    /// Gets the previous state of the screen.
    /// </summary>
    public ScreenState PreviousState { get; }
}
