namespace MvvmElegance.UnitTests.ConductorAllActiveTests;

public class ActivateItemAsyncTests
{
    [Theory]
    [AutoData]
    public async Task ActivateItemAsync_ActivatesScreen_WhenActive(
        Conductor<Screen>.Collection.AllActive conductor,
        Screen screen
    )
    {
        await ScreenExtensions.TryActivateAsync(conductor);

        await conductor.ActivateItemAsync(screen);

        screen.State.Should().Be(ScreenState.Active);
    }

    [Theory]
    [AutoData]
    public async Task ActivateItemAsync_DeactivatesScreen_WhenInactive(
        Conductor<Screen>.Collection.AllActive conductor,
        Screen screen
    )
    {
        await ScreenExtensions.TryActivateAsync(screen);

        await conductor.ActivateItemAsync(screen);

        screen.State.Should().Be(ScreenState.Inactive);
    }

    [Theory]
    [AutoData]
    public async Task ActivateItemAsync_AddsItem(Conductor<object>.Collection.AllActive conductor, object item)
    {
        await conductor.ActivateItemAsync(item);

        conductor.Items.Should().Contain(item);
    }

    [Theory]
    [AutoData]
    public async Task ActivateItemAsync_SetsScreenParent(
        Conductor<Screen>.Collection.AllActive conductor,
        Screen screen
    )
    {
        await conductor.ActivateItemAsync(screen);

        screen.Parent.Should().Be(conductor);
    }
}
