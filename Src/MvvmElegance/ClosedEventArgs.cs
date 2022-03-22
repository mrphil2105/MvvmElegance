namespace MvvmElegance;

public class ClosedEventArgs : ScreenStateEventArgs
{
    public ClosedEventArgs(ScreenState previousState) : base(previousState)
    {
    }
}
