using System.Collections.Specialized;

namespace MvvmElegance.UnitTests.BindableCollectionTests;

public class RemoveAllTests
{
    [Theory]
    [AutoData]
    public void RemoveAll_RemovesMatches_WhenRemovingEven(
        BindableCollection<int> collection,
        [CollectionSize(10)] List<int> items
    )
    {
        collection.AddRange(items);

        collection.RemoveAll(n => n % 2 == 0);

        collection.Should().BeEquivalentTo(items.Where(n => n % 2 == 1));
    }

    [Theory]
    [AutoData]
    public void RemoveAll_DoesNotRemoveMatches_WhenRemovingNone(
        BindableCollection<int> collection,
        [CollectionSize(10)] List<int> items
    )
    {
        collection.AddRange(items);

        collection.RemoveAll(_ => false);

        collection.Should().BeEquivalentTo(items);
    }

    [Theory]
    [AutoData]
    public void RemoveAll_RaisesCollectionChanged_WhenRemovingEven(
        BindableCollection<int> collection,
        [CollectionSize(10)] List<int> items
    )
    {
        collection.AddRange(items);
        using var monitor = collection.Monitor();

        collection.RemoveAll(n => n % 2 == 0);

        monitor
            .Should()
            .Raise(nameof(BindableCollection<int>.CollectionChanged))
            .WithArgs<NotifyCollectionChangedEventArgs>(e => e.Action == NotifyCollectionChangedAction.Reset);
    }

    [Theory]
    [AutoData]
    public void RemoveAll_DoesNotRaiseCollectionChanged_WhenRemovingNone(
        BindableCollection<int> collection,
        [CollectionSize(10)] List<int> items
    )
    {
        collection.AddRange(items);
        using var monitor = collection.Monitor();

        collection.RemoveAll(_ => false);

        monitor.Should().NotRaise(nameof(BindableCollection<int>.CollectionChanged));
    }
}
