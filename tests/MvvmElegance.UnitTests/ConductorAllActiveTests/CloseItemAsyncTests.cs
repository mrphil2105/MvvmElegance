using System.Reflection;
using Moq;

namespace MvvmElegance.UnitTests.ConductorAllActiveTests;

public class CloseItemAsyncTests
{
    [Theory]
    [AutoData]
    public async Task CloseItemAsync_ClosesScreen(Conductor<Screen>.Collection.AllActive conductor, Screen screen)
    {
        await conductor.CloseItemAsync(screen);

        screen.State.Should().Be(ScreenState.Closed);
    }

    [Theory]
    [AutoData]
    public async Task CloseItemAsync_ClearsScreenParent(Conductor<Screen>.Collection.AllActive conductor, Screen screen)
    {
        await conductor.ActivateItemAsync(screen);

        await conductor.CloseItemAsync(screen);

        screen.Parent.Should().BeNull();
    }

    [Theory]
    [AutoData]
    public async Task CloseItemAsync_RemovesItem(Conductor<Screen>.Collection.AllActive conductor, Screen screen)
    {
        await conductor.ActivateItemAsync(screen);

        await conductor.CloseItemAsync(screen);

        conductor.Items.Should().NotContain(screen);
    }

    [Theory]
    [AutoMoqData]
    public async Task CloseItemAsync_CallsDispose_WhenDisposeChildrenIsTrue(
        Mock<IDisposable> disposableMock,
        Conductor<IDisposable>.Collection.AllActive conductor
    )
    {
        await conductor.CloseItemAsync(disposableMock.Object);

        disposableMock.Verify(d => d.Dispose(), Times.Once);
    }

    [Theory]
    [AutoMoqData]
    public async Task CloseItemAsync_DoesNotCallDispose_WhenDisposeChildrenIsFalse(
        Mock<IDisposable> disposableMock,
        Conductor<IDisposable>.Collection.AllActive conductor
    )
    {
        var property = conductor
            .GetType()
            .GetProperty("DisposeChildren", BindingFlags.Instance | BindingFlags.NonPublic);
        property!.SetValue(conductor, false);

        await conductor.CloseItemAsync(disposableMock.Object);

        disposableMock.Verify(d => d.Dispose(), Times.Never);
    }
}
