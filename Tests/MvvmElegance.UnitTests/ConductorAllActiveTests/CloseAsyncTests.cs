namespace MvvmElegance.UnitTests.ConductorAllActiveTests;

public class CloseAsyncTests
{
    [Theory]
    [AutoData]
    public async Task CloseAsync_ClosesItems(Conductor<Screen>.Collection.AllActive conductor, List<Screen> screens)
    {
        conductor.Items.AddRange(screens);

        await ScreenExtensions.TryCloseAsync(conductor);

        screens.Should()
            .OnlyContain(s => s.State == ScreenState.Closed);
    }

    [Theory]
    [AutoData]
    public async Task CloseAsync_ClearsItemsParent(Conductor<Screen>.Collection.AllActive conductor,
        List<Screen> screens)
    {
        conductor.Items.AddRange(screens);

        await ScreenExtensions.TryCloseAsync(conductor);

        screens.Should()
            .OnlyContain(s => s.Parent == null);
    }

    [Theory]
    [AutoData]
    public async Task CloseAsync_ClearsItems(Conductor<object>.Collection.AllActive conductor,
        IEnumerable<object> items)
    {
        conductor.Items.AddRange(items);

        await ScreenExtensions.TryCloseAsync(conductor);

        conductor.Items.Should()
            .BeEmpty();
    }
}
