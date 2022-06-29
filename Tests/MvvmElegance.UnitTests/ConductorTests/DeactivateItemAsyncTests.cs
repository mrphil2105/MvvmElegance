namespace MvvmElegance.UnitTests.ConductorTests;

public class DeactivateItemAsyncTests
{
    [Theory]
    [AutoData]
    public async Task DeactivateItemAsync_DeactivatesActiveItem(Conductor<Screen> conductor)
    {
        await ScreenExtensions.TryActivateAsync(conductor.ActiveItem);

        await conductor.DeactivateItemAsync(conductor.ActiveItem);

        conductor.ActiveItem!.State.Should()
            .Be(ScreenState.Inactive);
    }

    [Theory]
    [AutoData]
    public async Task DeactivateItemAsync_DoesNotDeactivateScreen(Conductor<Screen> conductor, Screen screen)
    {
        await ScreenExtensions.TryActivateAsync(screen);

        await conductor.DeactivateItemAsync(screen);

        screen.State.Should()
            .Be(ScreenState.Active);
    }

    [Theory]
    [AutoData]
    public async Task DeactivateItemAsync_DoesNotSetScreenParent(Conductor<Screen> conductor, Screen screen)
    {
        await conductor.DeactivateItemAsync(screen);

        screen.Parent.Should()
            .NotBe(conductor);
    }
}
