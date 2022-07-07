namespace MvvmElegance.UnitTests.ConductorOneActiveTests;

public class ActivateItemAsyncTests
{
    [Theory]
    [AutoData]
    public async Task ActivateItemAsync_SetsActiveItem(Conductor<object>.Collection.OneActive conductor, object item)
    {
        await conductor.ActivateItemAsync(item);

        conductor.ActiveItem.Should()
            .Be(item);
    }

    [Theory]
    [AutoData]
    public async Task ActivateItemAsync_ActivatesActiveItem_WhenActive(Conductor<Screen>.Collection.OneActive conductor)
    {
        await ScreenExtensions.TryActivateAsync(conductor);
        await ScreenExtensions.TryDeactivateAsync(conductor.ActiveItem);

        await conductor.ActivateItemAsync(conductor.ActiveItem);

        conductor.ActiveItem!.State.Should()
            .Be(ScreenState.Active);
    }

    [Theory]
    [AutoData]
    public async Task ActivateItemAsync_DeactivatesActiveItem_WhenInactive(
        Conductor<Screen>.Collection.OneActive conductor)
    {
        await ScreenExtensions.TryActivateAsync(conductor.ActiveItem);

        await conductor.ActivateItemAsync(conductor.ActiveItem);

        conductor.ActiveItem!.State.Should()
            .Be(ScreenState.Inactive);
    }

    [Theory]
    [AutoData]
    public async Task ActivateItemAsync_ClearsActiveItem_WhenGivenNull(Conductor<object>.Collection.OneActive conductor)
    {
        await conductor.ActivateItemAsync(null);

        conductor.ActiveItem.Should()
            .BeNull();
    }

    [Theory]
    [AutoData]
    public async Task ActivateItemAsync_ActivatesScreen_WhenActive(Conductor<Screen>.Collection.OneActive conductor,
        Screen screen)
    {
        await ScreenExtensions.TryActivateAsync(conductor);

        await conductor.ActivateItemAsync(screen);

        screen.State.Should()
            .Be(ScreenState.Active);
    }

    [Theory]
    [AutoData]
    public async Task ActivateItemAsync_DeactivatesScreen_WhenInactive(Conductor<Screen>.Collection.OneActive conductor,
        Screen screen)
    {
        await ScreenExtensions.TryActivateAsync(screen);

        await conductor.ActivateItemAsync(screen);

        screen.State.Should()
            .Be(ScreenState.Inactive);
    }

    [Theory]
    [AutoData]
    public async Task ActivateItemAsync_AddsItem(Conductor<object>.Collection.OneActive conductor, object item)
    {
        await conductor.ActivateItemAsync(item);

        conductor.Items.Should()
            .Contain(item);
    }

    [Theory]
    [AutoData]
    public async Task ActivateItemAsync_SetsScreenParent(Conductor<Screen>.Collection.OneActive conductor,
        Screen screen)
    {
        await conductor.ActivateItemAsync(screen);

        screen.Parent.Should()
            .Be(conductor);
    }
}
