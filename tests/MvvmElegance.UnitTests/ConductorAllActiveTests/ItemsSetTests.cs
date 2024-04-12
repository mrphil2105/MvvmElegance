using System.Reflection;
using Moq;

namespace MvvmElegance.UnitTests.ConductorAllActiveTests;

public class ItemsSetTests
{
    [Theory]
    [AutoMoqData]
    public void ItemsSet_DoesNotSetScreenParent_WhenGivenSameScreen(
        Mock<Screen> screenMock,
        Conductor<Screen>.Collection.AllActive conductor
    )
    {
        conductor.Items.Add(screenMock.Object);
        screenMock.Invocations.Clear();

        conductor.Items[0] = screenMock.Object;

        screenMock.VerifySet(s => s.Parent = It.IsAny<object>(), Times.Never);
    }

    [Theory]
    [AutoMoqData]
    public void ItemsSet_CallsDispose_WhenDisposeChildrenIsTrue(
        Mock<IDisposable> disposableMock,
        Conductor<IDisposable>.Collection.AllActive conductor,
        IDisposable newDisposable
    )
    {
        conductor.Items.Add(disposableMock.Object);

        conductor.Items[0] = newDisposable;

        disposableMock.Verify(d => d.Dispose(), Times.Once);
    }

    [Theory]
    [AutoMoqData]
    public void ItemsSet_DoesNotCallDispose_WhenDisposeChildrenIsFalse(
        Mock<IDisposable> disposableMock,
        Conductor<IDisposable>.Collection.AllActive conductor,
        IDisposable newDisposable
    )
    {
        var property = conductor
            .GetType()
            .GetProperty("DisposeChildren", BindingFlags.Instance | BindingFlags.NonPublic);
        property!.SetValue(conductor, false);
        conductor.Items.Add(disposableMock.Object);

        conductor.Items[0] = newDisposable;

        disposableMock.Verify(d => d.Dispose(), Times.Never);
    }

    [Theory]
    [AutoMoqData]
    public void ItemsSet_DoesNotCallDispose_WhenGivenSameDisposable(
        Mock<IDisposable> disposableMock,
        Conductor<IDisposable>.Collection.AllActive conductor
    )
    {
        conductor.Items.Add(disposableMock.Object);

        conductor.Items[0] = disposableMock.Object;

        disposableMock.Verify(d => d.Dispose(), Times.Never);
    }
}
