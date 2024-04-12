using Moq;

namespace MvvmElegance.UnitTests.EventAggregatorTests;

public class PublishTests
{
    [Theory]
    [AutoMoqData]
    public void Publish_RaisesEvent_WhenSubscribed(
        EventAggregator eventAggregator,
        Mock<Action<IEvent>> delegateMock,
        Mock<IEvent> eventMock
    )
    {
        eventAggregator.Subscribe(delegateMock.Object, false);

        eventAggregator.Publish(eventMock.Object);

        delegateMock.Verify(d => d(eventMock.Object), Times.Once);
    }

    [Theory]
    [AutoMoqData]
    public void Publish_DoesNotRaiseEvent_WhenUnsubscribed(
        EventAggregator eventAggregator,
        Mock<Action<IEvent>> delegateMock,
        Mock<IEvent> eventMock
    )
    {
        var unsubscriber = eventAggregator.Subscribe(delegateMock.Object, false);
        unsubscriber.Dispose();

        eventAggregator.Publish(eventMock.Object);

        delegateMock.Verify(d => d(eventMock.Object), Times.Never);
    }
}
