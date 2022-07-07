using System.Reflection;
using Moq;

namespace MvvmElegance.UnitTests.ConductorOneActiveTests;

public class ItemsRemoveTests
{
    [Theory]
    [AutoData]
    public void ItemsRemove_ClosesScreen_WhenContainsScreen(Conductor<Screen>.Collection.OneActive conductor,
        Screen screen)
    {
        conductor.Items.Add(screen);

        conductor.Items.Remove(screen);

        screen.State.Should()
            .Be(ScreenState.Closed);
    }

    [Theory]
    [AutoData]
    public void ItemsRemove_ClearsScreenParent_WhenContainsScreen(Conductor<Screen>.Collection.OneActive conductor,
        Screen screen)
    {
        conductor.Items.Add(screen);

        conductor.Items.Remove(screen);

        screen.Parent.Should()
            .BeNull();
    }

    [Theory]
    [AutoMoqData]
    public void ItemsRemove_CallsDispose_WhenDisposeChildrenIsTrue([Frozen] Mock<IDisposable> disposableMock,
        Conductor<IDisposable>.Collection.OneActive conductor)
    {
        conductor.Items.Remove(conductor.ActiveItem!);

        disposableMock.Verify(d => d.Dispose(), Times.Once);
    }

    [Theory]
    [AutoMoqData]
    public void ItemsRemove_DoesNotCallDispose_WhenDisposeChildrenIsFalse([Frozen] Mock<IDisposable> disposableMock,
        Conductor<IDisposable>.Collection.OneActive conductor)
    {
        var property = conductor.GetType()
            .GetProperty("DisposeChildren", BindingFlags.Instance | BindingFlags.NonPublic);
        property!.SetValue(conductor, false);

        conductor.Items.Remove(conductor.ActiveItem!);

        disposableMock.Verify(d => d.Dispose(), Times.Never);
    }

    [Theory]
    [AutoData]
    public void ItemsRemove_ClearsActiveItem_WhenContainsSingle(Conductor<object>.Collection.OneActive conductor)
    {
        conductor.Items.Remove(conductor.ActiveItem!);

        conductor.ActiveItem.Should()
            .BeNull();
    }

    [Theory]
    [AutoData]
    public async Task ItemsRemove_ChangesActiveItem_WhenContainsMultiple(
        Conductor<object>.Collection.OneActive conductor, List<object> items)
    {
        conductor.Items.AddRange(items);
        await conductor.ActivateItemAsync(items.Last());

        conductor.Items.Remove(items.Last());

        conductor.ActiveItem.Should()
            .Be(conductor.Items[^1]);
    }
}
