using System.Reflection;
using Moq;

namespace MvvmElegance.UnitTests.ConductorOneActiveTests;

public class ItemsSetTests
{
    [Theory]
    [AutoMoqData]
    public void ItemsSet_DoesNotSetScreenParent_WhenGivenSameScreen(Mock<Screen> screenMock,
        Conductor<Screen>.Collection.OneActive conductor)
    {
        conductor.Items[0] = screenMock.Object;
        screenMock.Invocations.Clear();

        conductor.Items[0] = screenMock.Object;

        screenMock.VerifySet(s => s.Parent = It.IsAny<object>(), Times.Never);
    }

    [Theory]
    [AutoMoqData]
    public void ItemsSet_CallsDispose_WhenDisposeChildrenIsTrue(Mock<IDisposable> disposableMock,
        Conductor<IDisposable>.Collection.OneActive conductor, IDisposable newDisposable)
    {
        conductor.Items[0] = disposableMock.Object;

        conductor.Items[0] = newDisposable;

        disposableMock.Verify(d => d.Dispose(), Times.Once);
    }

    [Theory]
    [AutoMoqData]
    public void ItemsSet_DoesNotCallDispose_WhenDisposeChildrenIsFalse(Mock<IDisposable> disposableMock,
        Conductor<IDisposable>.Collection.OneActive conductor, IDisposable newDisposable)
    {
        var property = conductor.GetType()
            .GetProperty("DisposeChildren", BindingFlags.Instance | BindingFlags.NonPublic);
        property!.SetValue(conductor, false);
        conductor.Items[0] = disposableMock.Object;

        conductor.Items[0] = newDisposable;

        disposableMock.Verify(d => d.Dispose(), Times.Never);
    }

    [Theory]
    [AutoMoqData]
    public void ItemsSet_DoesNotCallDispose_WhenGivenSameDisposable([Frozen] Mock<IDisposable> disposableMock,
        Conductor<IDisposable>.Collection.OneActive conductor)
    {
        conductor.Items[0] = disposableMock.Object;

        disposableMock.Verify(d => d.Dispose(), Times.Never);
    }

    [Theory]
    [AutoData]
    public void ItemsSet_ChangesActiveItem_WhenContainsSingle(Conductor<object>.Collection.OneActive conductor,
        object newItem)
    {
        conductor.Items[0] = newItem;

        conductor.ActiveItem.Should()
            .Be(newItem);
    }

    [Theory]
    [AutoData]
    public async Task ItemsSet_ChangesActiveItem_WhenContainsMultiple(Conductor<object>.Collection.OneActive conductor,
        List<object> items, object newItem)
    {
        conductor.Items.AddRange(items);
        await conductor.ActivateItemAsync(items.Last());

        conductor.Items[^1] = newItem;

        conductor.ActiveItem.Should()
            .Be(conductor.Items[^2]);
    }
}
