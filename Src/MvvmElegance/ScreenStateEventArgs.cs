namespace MvvmElegance;

public abstract class ScreenStateEventArgs : EventArgs
{
    protected ScreenStateEventArgs(ScreenState previousState)
    {
        PreviousState = previousState;
    }

    public ScreenState PreviousState { get; }
}
