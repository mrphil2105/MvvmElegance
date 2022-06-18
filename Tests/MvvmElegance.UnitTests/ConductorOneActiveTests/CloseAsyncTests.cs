namespace MvvmElegance.UnitTests.ConductorOneActiveTests;

public class CloseAsyncTests
{
    [Theory]
    [AutoData]
    public async Task CloseAsync_ClosesActiveItem(Conductor<Screen>.Collection.OneActive conductor)
    {
        var activeItem = conductor.ActiveItem!;

        await ScreenExtensions.TryCloseAsync(conductor);

        activeItem.State.Should()
            .Be(ScreenState.Closed);
    }

    [Theory]
    [AutoData]
    public async Task CloseAsync_ClearsActiveItem(Conductor<Screen>.Collection.OneActive conductor)
    {
        await ScreenExtensions.TryCloseAsync(conductor);

        conductor.ActiveItem.Should()
            .BeNull();
    }

    [Theory]
    [AutoData]
    public async Task CloseAsync_ClosesItems(Conductor<Screen>.Collection.OneActive conductor, List<Screen> screens)
    {
        conductor.Items.AddRange(screens);

        await ScreenExtensions.TryCloseAsync(conductor);

        screens.Should()
            .OnlyContain(s => s.State == ScreenState.Closed);
    }

    [Theory]
    [AutoData]
    public async Task CloseAsync_ClearsItems(Conductor<object>.Collection.OneActive conductor,
        IEnumerable<object> items)
    {
        conductor.Items.Add(items);

        await ScreenExtensions.TryCloseAsync(conductor);

        conductor.Items.Should()
            .BeEmpty();
    }
}
