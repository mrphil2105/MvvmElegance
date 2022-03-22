namespace MvvmElegance;

public class ActivatedEventArgs : ScreenStateEventArgs
{
    public ActivatedEventArgs(bool wasInitialized, ScreenState previousState) : base(previousState)
    {
        WasInitialized = wasInitialized;
    }

    public bool WasInitialized { get; }
}
