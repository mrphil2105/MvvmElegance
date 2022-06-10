using System.Collections.Specialized;

namespace MvvmElegance.UnitTests.BindableCollectionTests;

public class AddRangeTests
{
    [Theory]
    [AutoData]
    public void AddRange_AddsRange_WhenAddingNonEmpty(BindableCollection<string> collection, List<string> items)
    {
        collection.AddRange(items);

        collection.Should()
            .BeEquivalentTo(items);
    }

    [Theory]
    [AutoData]
    public void AddRange_DoesNotAddRange_WhenAddingEmpty(BindableCollection<string> collection)
    {
        collection.AddRange(Enumerable.Empty<string>());

        collection.Should()
            .BeEmpty();
    }

    [Theory]
    [AutoData]
    public void AddRange_RaisesCollectionChanged_WhenAddingNonEmpty(BindableCollection<string> collection,
        List<string> items)
    {
        using var monitor = collection.Monitor();

        collection.AddRange(items);

        monitor.Should()
            .Raise(nameof(BindableCollection<string>.CollectionChanged))
            .WithArgs<NotifyCollectionChangedEventArgs>(e => e.Action == NotifyCollectionChangedAction.Reset);
    }

    [Theory]
    [AutoData]
    public void AddRange_DoesNotRaiseCollectionChanged_WhenAddingEmpty(BindableCollection<string> collection)
    {
        using var monitor = collection.Monitor();

        collection.AddRange(Enumerable.Empty<string>());

        monitor.Should()
            .NotRaise(nameof(BindableCollection<string>.CollectionChanged));
    }
}
