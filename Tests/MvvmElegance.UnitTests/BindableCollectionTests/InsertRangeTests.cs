using System.Collections.Specialized;

namespace MvvmElegance.UnitTests.BindableCollectionTests;

public class InsertRangeTests
{
    [Theory]
    [AutoData]
    public void InsertRange_InsertsRange_WhenAddingNonEmpty(BindableCollection<string> collection, List<string> items,
        List<string> additionalItems)
    {
        const int index = 2;
        collection.AddRange(items);

        collection.InsertRange(index, additionalItems);

        collection.Should()
            .BeEquivalentTo(items.Take(index)
                .Concat(additionalItems)
                .Concat(items.Skip(index)));
    }

    [Theory]
    [AutoData]
    public void InsertRange_DoesNotInsertRange_WhenAddingEmpty(BindableCollection<string> collection,
        List<string> items)
    {
        const int index = 2;
        collection.AddRange(items);

        collection.InsertRange(index, Enumerable.Empty<string>());

        collection.Should()
            .BeEquivalentTo(items);
    }

    [Theory]
    [AutoData]
    public void InsertRange_RaisesCollectionChanged_WhenAddingNonEmpty(BindableCollection<string> collection,
        List<string> items, List<string> additionalItems)
    {
        const int index = 2;
        collection.AddRange(items);
        using var monitor = collection.Monitor();

        collection.InsertRange(index, additionalItems);

        monitor.Should()
            .Raise(nameof(BindableCollection<string>.CollectionChanged))
            .WithArgs<NotifyCollectionChangedEventArgs>(e => e.Action == NotifyCollectionChangedAction.Reset);
    }

    [Theory]
    [AutoData]
    public void InsertRange_DoesNotRaiseCollectionChanged_WhenAddingEmpty(BindableCollection<string> collection,
        List<string> items)
    {
        const int index = 2;
        collection.AddRange(items);
        using var monitor = collection.Monitor();

        collection.InsertRange(index, Enumerable.Empty<string>());

        monitor.Should()
            .NotRaise(nameof(BindableCollection<string>.CollectionChanged));
    }
}
