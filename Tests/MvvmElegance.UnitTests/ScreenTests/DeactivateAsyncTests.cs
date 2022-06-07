namespace MvvmElegance.UnitTests.ScreenTests;

public class DeactivateAsyncTests
{
    [Theory]
    [AutoData]
    public async Task DeactivateAsync_DeactivatesScreen_WhenActive(Screen screen)
    {
        await ScreenExtensions.TryActivateAsync(screen);

        await ScreenExtensions.TryDeactivateAsync(screen);

        screen.State.Should()
            .Be(ScreenState.Inactive);
    }

    [Theory]
    [AutoData]
    public async Task DeactivateAsync_RaisesDeactivated_WhenActive(Screen screen)
    {
        await ScreenExtensions.TryActivateAsync(screen);
        using var monitor = screen.Monitor();

        await ScreenExtensions.TryDeactivateAsync(screen);

        monitor.Should()
            .Raise(nameof(Screen.Deactivated));
    }

    [Theory]
    [AutoData]
    public async Task DeactivateAsync_RaisesActivatedAndDeactivated_WhenClosed(Screen screen)
    {
        await ScreenExtensions.TryCloseAsync(screen);
        using var monitor = screen.Monitor();

        await ScreenExtensions.TryDeactivateAsync(screen);

        monitor.Should()
            .Raise(nameof(Screen.Activated));
        monitor.Should()
            .Raise(nameof(Screen.Deactivated));
    }

    [Theory]
    [AutoData]
    public async Task DeactivateAsync_DoesNotRaiseDeactivated_WhenAlreadyInactive(Screen screen)
    {
        using var monitor = screen.Monitor();

        await ScreenExtensions.TryDeactivateAsync(screen);

        monitor.Should()
            .NotRaise(nameof(Screen.Deactivated));
    }
}
