using System.Collections.Specialized;

namespace MvvmElegance.UnitTests.BindableCollectionTests;

public class RemoveRangeTests
{
    [Theory]
    [AutoData]
    public void RemoveRange_RemovesRange_WhenRemovingRange(BindableCollection<string> collection,
        [CollectionSize(6)] List<string> items)
    {
        const int index = 2;
        const int count = 3;
        collection.AddRange(items);

        collection.RemoveRange(index, count);

        collection.Should()
            .BeEquivalentTo(items.Where((_, i) => i is < index or >= index + count));
    }

    [Theory]
    [AutoData]
    public void RemoveRange_DoesNotRemoveRange_WhenRemovingNone(BindableCollection<string> collection,
        [CollectionSize(6)] List<string> items)
    {
        const int index = 2;
        const int count = 0;
        collection.AddRange(items);

        collection.RemoveRange(index, count);

        collection.Should()
            .BeEquivalentTo(items);
    }

    [Theory]
    [AutoData]
    public void RemoveRange_RaisesCollectionChanged_WhenRemovingRange(BindableCollection<string> collection,
        [CollectionSize(6)] List<string> items)
    {
        const int index = 2;
        const int count = 3;
        collection.AddRange(items);
        using var monitor = collection.Monitor();

        collection.RemoveRange(index, count);

        monitor.Should()
            .Raise(nameof(BindableCollection<string>.CollectionChanged))
            .WithArgs<NotifyCollectionChangedEventArgs>(e => e.Action == NotifyCollectionChangedAction.Reset);
    }

    [Theory]
    [AutoData]
    public void RemoveRange_DoesNotRaiseCollectionChanged_WhenRemovingNone(BindableCollection<string> collection,
        [CollectionSize(6)] List<string> items)
    {
        const int index = 2;
        const int count = 0;
        collection.AddRange(items);
        using var monitor = collection.Monitor();

        collection.RemoveRange(index, count);

        monitor.Should()
            .NotRaise(nameof(BindableCollection<string>.CollectionChanged));
    }
}
