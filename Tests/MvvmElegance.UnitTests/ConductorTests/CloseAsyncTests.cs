using System.Reflection;
using Moq;

namespace MvvmElegance.UnitTests.ConductorTests;

public class CloseAsyncTests
{
    [Theory]
    [AutoData]
    public async Task CloseAsync_ClosesActiveItem(Conductor<Screen> conductor)
    {
        await ScreenExtensions.TryCloseAsync(conductor);

        conductor.ActiveItem!.State.Should()
            .Be(ScreenState.Closed);
    }

    [Theory]
    [AutoData]
    public async Task CloseAsync_ClearsActiveItemParent(Conductor<Screen> conductor)
    {
        await ScreenExtensions.TryCloseAsync(conductor);

        conductor.ActiveItem!.Parent.Should()
            .BeNull();
    }

    [Theory]
    [AutoMoqData]
    public async Task CloseAsync_CallsDispose_WhenDisposeChildrenIsTrue([Frozen] Mock<IDisposable> disposableMock,
        Conductor<IDisposable> conductor)
    {
        await ScreenExtensions.TryCloseAsync(conductor);

        disposableMock.Verify(d => d.Dispose(), Times.Once);
    }

    [Theory]
    [AutoMoqData]
    public async Task CloseAsync_DoesNotCallDispose_WhenDisposeChildrenIsFalse(
        [Frozen] Mock<IDisposable> disposableMock, Conductor<IDisposable> conductor)
    {
        var property = conductor.GetType()
            .GetProperty("DisposeChildren", BindingFlags.Instance | BindingFlags.NonPublic);
        property!.SetValue(conductor, false);

        await ScreenExtensions.TryCloseAsync(conductor);

        disposableMock.Verify(d => d.Dispose(), Times.Never);
    }
}
