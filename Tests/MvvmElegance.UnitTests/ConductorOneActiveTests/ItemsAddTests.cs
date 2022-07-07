namespace MvvmElegance.UnitTests.ConductorOneActiveTests;

public class ItemsAddTests
{
    [Theory]
    [AutoData]
    public async Task ItemsAdd_DeactivatesScreen(Conductor<Screen>.Collection.OneActive conductor, Screen screen)
    {
        await ScreenExtensions.TryActivateAsync(screen);

        conductor.Items.Add(screen);

        screen.State.Should()
            .Be(ScreenState.Inactive);
    }

    [Theory]
    [AutoData]
    public void ItemsAdd_SetsScreenParent(Conductor<Screen>.Collection.OneActive conductor, Screen screen)
    {
        conductor.Items.Add(screen);

        screen.Parent.Should()
            .Be(conductor);
    }
}
