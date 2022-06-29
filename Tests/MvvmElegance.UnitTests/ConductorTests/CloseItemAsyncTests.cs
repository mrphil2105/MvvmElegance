using System.Reflection;
using Moq;

namespace MvvmElegance.UnitTests.ConductorTests;

public class CloseItemAsyncTests
{
    [Theory]
    [AutoData]
    public async Task CloseItemAsync_ClosesActiveItem(Conductor<Screen> conductor)
    {
        var activeItem = conductor.ActiveItem!;

        await conductor.CloseItemAsync(conductor.ActiveItem);

        activeItem.State.Should()
            .Be(ScreenState.Closed);
    }

    [Theory]
    [AutoData]
    public async Task CloseItemAsync_DoesNotCloseScreen(Conductor<Screen> conductor, Screen screen)
    {
        await conductor.CloseItemAsync(screen);

        screen.State.Should()
            .Be(ScreenState.Inactive);
    }

    [Theory]
    [AutoData]
    public async Task CloseItemAsync_ClearsScreenParent(Conductor<Screen> conductor, Screen screen)
    {
        await conductor.ActivateItemAsync(screen);

        await conductor.CloseItemAsync(screen);

        screen.Parent.Should()
            .BeNull();
    }

    [Theory]
    [AutoMoqData]
    public async Task CloseItemAsync_CallsDispose_WhenDisposeChildrenIsTrue([Frozen] Mock<IDisposable> disposableMock,
        Conductor<IDisposable> conductor)
    {
        await conductor.CloseItemAsync(conductor.ActiveItem);

        disposableMock.Verify(d => d.Dispose(), Times.Once);
    }

    [Theory]
    [AutoMoqData]
    public async Task CloseItemAsync_DoesNotCallDispose_WhenDisposeChildrenIsFalse(
        [Frozen] Mock<IDisposable> disposableMock, Conductor<IDisposable> conductor)
    {
        var property = conductor.GetType()
            .GetProperty("DisposeChildren", BindingFlags.Instance | BindingFlags.NonPublic);
        property!.SetValue(conductor, false);

        await conductor.CloseItemAsync(conductor.ActiveItem);

        disposableMock.Verify(d => d.Dispose(), Times.Never);
    }

    [Theory]
    [AutoData]
    public async Task CloseItemAsync_ClearsActiveItem(Conductor<object> conductor)
    {
        await conductor.CloseItemAsync(conductor.ActiveItem);

        conductor.ActiveItem.Should()
            .BeNull();
    }
}
