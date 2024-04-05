using System.Reflection;
using Moq;

namespace MvvmElegance.UnitTests.ConductorAllActiveTests;

public class ItemsRemoveTests
{
    [Theory]
    [AutoData]
    public void ItemsRemove_ClosesScreen_WhenContainsScreen(
        Conductor<Screen>.Collection.AllActive conductor,
        Screen screen
    )
    {
        conductor.Items.Add(screen);

        conductor.Items.Remove(screen);

        screen.State.Should().Be(ScreenState.Closed);
    }

    [Theory]
    [AutoData]
    public void ItemsRemove_ClearsScreenParent_WhenContainsScreen(
        Conductor<Screen>.Collection.AllActive conductor,
        Screen screen
    )
    {
        conductor.Items.Add(screen);

        conductor.Items.Remove(screen);

        screen.Parent.Should().BeNull();
    }

    [Theory]
    [AutoMoqData]
    public void ItemsRemove_CallsDispose_WhenDisposeChildrenIsTrue(
        Mock<IDisposable> disposableMock,
        Conductor<IDisposable>.Collection.AllActive conductor
    )
    {
        conductor.Items.Add(disposableMock.Object);

        conductor.Items.Remove(disposableMock.Object);

        disposableMock.Verify(d => d.Dispose(), Times.Once);
    }

    [Theory]
    [AutoMoqData]
    public void ItemsRemove_DoesNotCallDispose_WhenDisposeChildrenIsFalse(
        Mock<IDisposable> disposableMock,
        Conductor<IDisposable>.Collection.AllActive conductor
    )
    {
        var property = conductor
            .GetType()
            .GetProperty("DisposeChildren", BindingFlags.Instance | BindingFlags.NonPublic);
        property!.SetValue(conductor, false);
        conductor.Items.Add(disposableMock.Object);

        conductor.Items.Remove(disposableMock.Object);

        disposableMock.Verify(d => d.Dispose(), Times.Never);
    }
}
