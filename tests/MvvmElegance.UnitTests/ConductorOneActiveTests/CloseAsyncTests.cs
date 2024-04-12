using System.Reflection;
using Moq;

namespace MvvmElegance.UnitTests.ConductorOneActiveTests;

public class CloseAsyncTests
{
    [Theory]
    [AutoData]
    public async Task CloseAsync_ClosesActiveItem(Conductor<Screen>.Collection.OneActive conductor)
    {
        var activeItem = conductor.ActiveItem!;

        await ScreenExtensions.TryCloseAsync(conductor);

        activeItem.State.Should().Be(ScreenState.Closed);
    }

    [Theory]
    [AutoData]
    public async Task CloseAsync_ClearsActiveItem(Conductor<Screen>.Collection.OneActive conductor)
    {
        await ScreenExtensions.TryCloseAsync(conductor);

        conductor.ActiveItem.Should().BeNull();
    }

    [Theory]
    [AutoData]
    public async Task CloseAsync_ClosesItems(Conductor<Screen>.Collection.OneActive conductor, List<Screen> screens)
    {
        conductor.Items.AddRange(screens);

        await ScreenExtensions.TryCloseAsync(conductor);

        screens.Should().OnlyContain(s => s.State == ScreenState.Closed);
    }

    [Theory]
    [AutoData]
    public async Task CloseAsync_ClearsItemsParent(
        Conductor<Screen>.Collection.OneActive conductor,
        List<Screen> screens
    )
    {
        conductor.Items.AddRange(screens);

        await ScreenExtensions.TryCloseAsync(conductor);

        screens.Should().OnlyContain(s => s.Parent == null);
    }

    [Theory]
    [AutoData]
    public async Task CloseAsync_ClearsItems(
        Conductor<object>.Collection.OneActive conductor,
        IEnumerable<object> items
    )
    {
        conductor.Items.Add(items);

        await ScreenExtensions.TryCloseAsync(conductor);

        conductor.Items.Should().BeEmpty();
    }

    [Theory]
    [AutoMoqData]
    public async Task CloseAsync_CallsDispose_WhenDisposeChildrenIsTrue(
        List<Mock<IDisposable>> disposableMocks,
        Conductor<IDisposable>.Collection.OneActive conductor
    )
    {
        conductor.Items.AddRange(disposableMocks.Select(m => m.Object));

        await ScreenExtensions.TryCloseAsync(conductor);

        disposableMocks.Should().AllSatisfy(m => m.Verify(d => d.Dispose(), Times.Once));
    }

    [Theory]
    [AutoMoqData]
    public async Task CloseAsync_DoesNotCallDispose_WhenDisposeChildrenIsFalse(
        List<Mock<IDisposable>> disposableMocks,
        Conductor<IDisposable>.Collection.OneActive conductor
    )
    {
        var property = conductor
            .GetType()
            .GetProperty("DisposeChildren", BindingFlags.Instance | BindingFlags.NonPublic);
        property!.SetValue(conductor, false);
        conductor.Items.AddRange(disposableMocks.Select(m => m.Object));

        await ScreenExtensions.TryCloseAsync(conductor);

        disposableMocks.Should().AllSatisfy(m => m.Verify(d => d.Dispose(), Times.Never));
    }
}
