namespace MvvmElegance.UnitTests.ScreenTests;

public class ActivateAsyncTests
{
    [Theory]
    [AutoData]
    public async Task ActivateAsync_ActivatesScreen_WhenInactive(Screen screen)
    {
        await ScreenExtensions.TryActivateAsync(screen);

        screen.State.Should().Be(ScreenState.Active);
    }

    [Theory]
    [AutoData]
    public async Task ActivateAsync_InitializesScreen_WhenInitiallyInactive(Screen screen)
    {
        await ScreenExtensions.TryActivateAsync(screen);

        screen.IsInitialized.Should().BeTrue();
    }

    [Theory]
    [AutoData]
    public async Task ActivateAsync_RaisesActivated_WhenInactive(Screen screen)
    {
        using var monitor = screen.Monitor();

        await ScreenExtensions.TryActivateAsync(screen);

        monitor.Should().Raise(nameof(Screen.Activated));
    }

    [Theory]
    [AutoData]
    public async Task ActivateAsync_RaisesInitialized_WhenInitiallyInactive(Screen screen)
    {
        using var monitor = screen.Monitor();

        await ScreenExtensions.TryActivateAsync(screen);

        monitor.Should().Raise(nameof(Screen.Activated)).WithArgs<ActivatedEventArgs>(e => e.WasInitialized);
    }

    [Theory]
    [AutoData]
    public async Task ActivateAsync_DoesNotRaiseActivated_WhenAlreadyActive(Screen screen)
    {
        await ScreenExtensions.TryActivateAsync(screen);
        using var monitor = screen.Monitor();

        await ScreenExtensions.TryActivateAsync(screen);

        monitor.Should().NotRaise(nameof(Screen.Activated));
    }

    [Theory]
    [AutoData]
    public async Task ActivateAsync_DoesNotRaiseInitialized_WhenAlreadyInitialized(Screen screen)
    {
        await ScreenExtensions.TryActivateAsync(screen);
        await ScreenExtensions.TryDeactivateAsync(screen);
        using var monitor = screen.Monitor();

        await ScreenExtensions.TryActivateAsync(screen);

        monitor.Should().Raise(nameof(Screen.Activated)).WithArgs<ActivatedEventArgs>(e => !e.WasInitialized);
    }
}
