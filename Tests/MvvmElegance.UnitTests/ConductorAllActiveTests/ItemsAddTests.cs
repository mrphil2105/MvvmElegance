namespace MvvmElegance.UnitTests.ConductorAllActiveTests;

public class ItemsAddTests
{
    [Theory]
    [AutoData]
    public async Task ItemsAdd_ActivatesScreen_WhenActive(Conductor<Screen>.Collection.AllActive conductor,
        Screen screen)
    {
        await ScreenExtensions.TryActivateAsync(conductor);

        conductor.Items.Add(screen);

        screen.State.Should()
            .Be(ScreenState.Active);
    }

    [Theory]
    [AutoData]
    public async Task ItemsAdd_DeactivatesScreen_WhenInactive(Conductor<Screen>.Collection.AllActive conductor,
        Screen screen)
    {
        await ScreenExtensions.TryActivateAsync(screen);

        conductor.Items.Add(screen);

        screen.State.Should()
            .Be(ScreenState.Inactive);
    }

    [Theory]
    [AutoData]
    public void ItemsAdd_SetsScreenParent(Conductor<Screen>.Collection.AllActive conductor, Screen screen)
    {
        conductor.Items.Add(screen);

        screen.Parent.Should()
            .Be(conductor);
    }
}
