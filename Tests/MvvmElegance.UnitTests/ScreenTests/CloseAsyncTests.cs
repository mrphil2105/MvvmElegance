namespace MvvmElegance.UnitTests.ScreenTests;

public class CloseAsyncTests
{
    [Theory]
    [AutoData]
    public async Task CloseAsync_ClosesScreen_WhenInactive(Screen screen)
    {
        await ScreenExtensions.TryCloseAsync(screen);

        screen.State.Should().Be(ScreenState.Closed);
    }

    [Theory]
    [AutoData]
    public async Task CloseAsync_RaisesClosed_WhenInactive(Screen screen)
    {
        using var monitor = screen.Monitor();

        await ScreenExtensions.TryCloseAsync(screen);

        monitor.Should().Raise(nameof(Screen.Closed));
    }

    [Theory]
    [AutoData]
    public async Task CloseAsync_RaisesDeactivatedAndClosed_WhenActive(Screen screen)
    {
        await ScreenExtensions.TryActivateAsync(screen);
        using var monitor = screen.Monitor();

        await ScreenExtensions.TryCloseAsync(screen);

        monitor.Should().Raise(nameof(Screen.Deactivated));
        monitor.Should().Raise(nameof(Screen.Closed));
    }

    [Theory]
    [AutoData]
    public async Task CloseAsync_DoesNotRaiseClosed_WhenAlreadyClosed(Screen screen)
    {
        await ScreenExtensions.TryCloseAsync(screen);
        using var monitor = screen.Monitor();

        await ScreenExtensions.TryCloseAsync(screen);

        monitor.Should().NotRaise(nameof(Screen.Closed));
    }
}
