namespace MvvmElegance;

public static class ScreenExtensions
{
    public static Task TryActivateAsync(object? screen, CancellationToken cancellationToken = default)
    {
        return screen is IScreenState state ? state.ActivateAsync(cancellationToken) : Task.CompletedTask;
    }

    public static Task TryDeactivateAsync(object? screen, CancellationToken cancellationToken = default)
    {
        return screen is IScreenState state ? state.DeactivateAsync(cancellationToken) : Task.CompletedTask;
    }

    public static Task TryCloseAsync(object? screen, CancellationToken cancellationToken = default)
    {
        return screen is IScreenState state ? state.CloseAsync(cancellationToken) : Task.CompletedTask;
    }

    public static void TryDispose(object? screen)
    {
        if (screen is IDisposable disposable)
        {
            disposable.Dispose();
        }
    }
}
