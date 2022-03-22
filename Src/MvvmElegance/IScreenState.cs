namespace MvvmElegance;

public interface IScreenState
{
    ScreenState State { get; }

    event EventHandler<ActivatedEventArgs> Activated;

    event EventHandler<DeactivatedEventArgs> Deactivated;

    event EventHandler<ClosedEventArgs> Closed;

    Task ActivateAsync(CancellationToken cancellationToken = default);

    Task DeactivateAsync(CancellationToken cancellationToken = default);

    Task CloseAsync(CancellationToken cancellationToken = default);
}
