namespace MvvmElegance.UnitTests.ConductorAllActiveTests;

public class ItemsClearTests
{
    [Theory]
    [AutoData]
    public void ItemsClear_ClosesItems(Conductor<Screen>.Collection.AllActive conductor, List<Screen> screens)
    {
        conductor.Items.AddRange(screens);

        conductor.Items.Clear();

        screens.Should().OnlyContain(s => s.State == ScreenState.Closed);
    }

    [Theory]
    [AutoData]
    public void ItemsClear_ClearsItemsParent(Conductor<Screen>.Collection.AllActive conductor, List<Screen> screens)
    {
        conductor.Items.AddRange(screens);

        conductor.Items.Clear();

        screens.Should().OnlyContain(s => s.Parent == null);
    }
}
