using System.Reflection;
using Moq;

namespace MvvmElegance.UnitTests.ConductorOneActiveTests;

public class CloseItemAsyncTests
{
    [Theory]
    [AutoData]
    public async Task CloseItemAsync_ClosesActiveItem(Conductor<Screen>.Collection.OneActive conductor)
    {
        var activeItem = conductor.ActiveItem!;

        await conductor.CloseItemAsync(conductor.ActiveItem);

        activeItem.State.Should().Be(ScreenState.Closed);
    }

    [Theory]
    [AutoData]
    public async Task CloseItemAsync_ClosesScreen(Conductor<Screen>.Collection.OneActive conductor, Screen screen)
    {
        await conductor.CloseItemAsync(screen);

        screen.State.Should().Be(ScreenState.Closed);
    }

    [Theory]
    [AutoData]
    public async Task CloseItemAsync_ClearsScreenParent(Conductor<Screen>.Collection.OneActive conductor, Screen screen)
    {
        await conductor.ActivateItemAsync(screen);

        await conductor.CloseItemAsync(screen);

        screen.Parent.Should().BeNull();
    }

    [Theory]
    [AutoData]
    public async Task CloseItemAsync_RemovesItem(Conductor<Screen>.Collection.OneActive conductor, Screen screen)
    {
        await conductor.ActivateItemAsync(screen);

        await conductor.CloseItemAsync(screen);

        conductor.Items.Should().NotContain(screen);
    }

    [Theory]
    [AutoMoqData]
    public async Task CloseItemAsync_CallsDispose_WhenDisposeChildrenIsTrue(
        [Frozen] Mock<IDisposable> disposableMock,
        Conductor<IDisposable>.Collection.OneActive conductor
    )
    {
        await conductor.CloseItemAsync(conductor.ActiveItem);

        disposableMock.Verify(d => d.Dispose(), Times.Once);
    }

    [Theory]
    [AutoMoqData]
    public async Task CloseItemAsync_DoesNotCallDispose_WhenDisposeChildrenIsFalse(
        [Frozen] Mock<IDisposable> disposableMock,
        Conductor<IDisposable>.Collection.OneActive conductor
    )
    {
        var property = conductor
            .GetType()
            .GetProperty("DisposeChildren", BindingFlags.Instance | BindingFlags.NonPublic);
        property!.SetValue(conductor, false);

        await conductor.CloseItemAsync(conductor.ActiveItem);

        disposableMock.Verify(d => d.Dispose(), Times.Never);
    }

    [Theory]
    [AutoData]
    public async Task CloseItemAsync_ClearsActiveItem_WhenContainsSingle(
        Conductor<object>.Collection.OneActive conductor
    )
    {
        await conductor.CloseItemAsync(conductor.ActiveItem);

        conductor.ActiveItem.Should().BeNull();
    }

    [Theory]
    [AutoData]
    public async Task CloseItemAsync_ChangesActiveItem_WhenContainsMultiple(
        Conductor<object>.Collection.OneActive conductor,
        List<object> items
    )
    {
        conductor.Items.AddRange(items);
        await conductor.ActivateItemAsync(items.Last());

        await conductor.CloseItemAsync(items.Last());

        conductor.ActiveItem.Should().Be(conductor.Items[^1]);
    }
}
