namespace MvvmElegance.UnitTests.ConductorOneActiveTests;

public class ItemsClearTests
{
    [Theory]
    [AutoData]
    public void ItemsClear_ClearsActiveItem(Conductor<object>.Collection.OneActive conductor)
    {
        conductor.Items.Clear();

        conductor.ActiveItem.Should().BeNull();
    }

    [Theory]
    [AutoData]
    public void ItemsClear_ClosesItems(Conductor<Screen>.Collection.OneActive conductor, List<Screen> screens)
    {
        conductor.Items.AddRange(screens);

        conductor.Items.Clear();

        screens.Should().OnlyContain(s => s.State == ScreenState.Closed);
    }

    [Theory]
    [AutoData]
    public void ItemsClear_ClearsItemsParent(Conductor<Screen>.Collection.OneActive conductor, List<Screen> screens)
    {
        conductor.Items.AddRange(screens);

        conductor.Items.Clear();

        screens.Should().OnlyContain(s => s.Parent == null);
    }
}
