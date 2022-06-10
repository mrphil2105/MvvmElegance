namespace MvvmElegance;

public interface IConductor : IParent
{
    Task ActivateItemAsync(object? item, CancellationToken cancellationToken = default);

    Task DeactivateItemAsync(object? item, CancellationToken cancellationToken = default);

    Task CloseItemAsync(object? item, CancellationToken cancellationToken = default);
}
