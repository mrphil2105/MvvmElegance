namespace MvvmElegance;

public interface IGuardClose
{
    Task<bool> CanCloseAsync(CancellationToken cancellationToken = default);
}
