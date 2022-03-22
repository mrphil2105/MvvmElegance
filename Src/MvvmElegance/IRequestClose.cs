namespace MvvmElegance;

public interface IRequestClose
{
    Task<bool> TryCloseAsync(bool? dialogResult = null, CancellationToken cancellationToken = default);
}
