namespace MvvmElegance;

public class DeactivatedEventArgs : ScreenStateEventArgs
{
    public DeactivatedEventArgs(ScreenState previousState) : base(previousState)
    {
    }
}
