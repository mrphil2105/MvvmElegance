namespace MvvmElegance;

/// <summary>
/// Provides event arguments for when a screen has been activated.
/// </summary>
public class ActivatedEventArgs : ScreenStateEventArgs
{
    /// <summary>
    /// Initializes a new instance of the <see cref="ActivatedEventArgs" /> class with the specified state parameters.
    /// </summary>
    /// <param name="wasInitialized">A boolean indicating whether the screen was initialized during the activation.</param>
    /// <param name="previousState">The previous state of the screen.</param>
    public ActivatedEventArgs(bool wasInitialized, ScreenState previousState) : base(previousState)
    {
        WasInitialized = wasInitialized;
    }

    /// <summary>
    /// Gets a boolean indicating whether the screen was initialized during the activation.
    /// </summary>
    public bool WasInitialized { get; }
}
