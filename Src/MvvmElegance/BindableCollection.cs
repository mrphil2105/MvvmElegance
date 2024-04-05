using System.Collections.ObjectModel;
using System.Collections.Specialized;
using System.ComponentModel;

namespace MvvmElegance;

/// <summary>
/// Provides a collection that notifies when it is changed.
/// </summary>
/// <typeparam name="T">The type of elements in the collection.</typeparam>
public class BindableCollection<T> : ObservableCollection<T>
{
    private bool _shouldNotify;

    /// <summary>
    /// Initializes a new instance of the <see cref="BindableCollection{T}" /> class.
    /// </summary>
    public BindableCollection()
    {
        _shouldNotify = true;
    }

    /// <summary>
    /// Initializes a new instance of the <see cref="BindableCollection{T}" /> class with the elements of the specified collection.
    /// </summary>
    /// <param name="items">The collection with the elements to add.</param>
    public BindableCollection(IEnumerable<T> items)
        : base(items)
    {
        _shouldNotify = true;
    }

    internal event EventHandler? BeforeReset;

    // Used to inform conductors that no reset event is being raised, even though 'BeforeReset' was raised.
    // This allows collection conductors to clear their '_itemsBeforeReset' list.
    internal event EventHandler? NoReset;

    /// <summary>
    /// Adds the elements of the specified collection to the end of the <see cref="BindableCollection{T}" />.
    /// </summary>
    /// <param name="items">The collection with the elements to add.</param>
    /// <exception cref="ArgumentNullException">Value of parameter <paramref name="items" /> is <c>null</c>.</exception>
    public void AddRange(IEnumerable<T> items)
    {
        InsertRange(Count, items);
    }

    /// <summary>
    /// Inserts the elements of the specified collection at the specified index of the <see cref="BindableCollection{T}" />.
    /// </summary>
    /// <param name="index">The index to insert at.</param>
    /// <param name="items">The collection with the elements to add.</param>
    /// <exception cref="ArgumentOutOfRangeException">Value of parameter <paramref name="index" /> is negative or out of bounds.</exception>
    /// <exception cref="ArgumentNullException">Value of parameter <paramref name="items" /> is <c>null</c>.</exception>
    public void InsertRange(int index, IEnumerable<T> items)
    {
        if (index < 0)
        {
            throw new ArgumentOutOfRangeException(nameof(index), "Value cannot be negative.");
        }

        if (index > Count)
        {
            throw new ArgumentOutOfRangeException(nameof(index), "Value is out of bounds.");
        }

        if (items is null)
        {
            throw new ArgumentNullException(nameof(items));
        }

        BeforeReset?.Invoke(this, EventArgs.Empty);

        var previousShouldNotify = _shouldNotify;
        _shouldNotify = false;

        var startingIndex = index;

        foreach (var item in items)
        {
            InsertItem(index, item);
            index++;
        }

        _shouldNotify = previousShouldNotify;

        if (index == startingIndex)
        {
            NoReset?.Invoke(this, EventArgs.Empty);

            return;
        }

        OnPropertyChanged(new PropertyChangedEventArgs("Count"));
        OnPropertyChanged(new PropertyChangedEventArgs("Item[]"));
        // Raising 'CollectionChanged' with ranges causes exceptions in the view.
        OnCollectionChanged(new NotifyCollectionChangedEventArgs(NotifyCollectionChangedAction.Reset));
    }

    /// <summary>
    /// Removes a range of elements from the <see cref="BindableCollection{T}" />.
    /// </summary>
    /// <param name="index">The starting index of the elements to remove.</param>
    /// <param name="count">The number of elements to remove.</param>
    /// <exception cref="ArgumentOutOfRangeException">Value of parameter <paramref name="index" /> or <paramref name="count" /> is negative.</exception>
    /// <exception cref="ArgumentException">
    /// Sum of the values of parameters <paramref name="index" /> and <paramref name="count" /> is larger than the number of elements in the
    /// <see cref="BindableCollection{T}" />.
    /// </exception>
    public void RemoveRange(int index, int count)
    {
        if (index < 0)
        {
            throw new ArgumentOutOfRangeException(nameof(index), "Value cannot be negative.");
        }

        if (count < 0)
        {
            throw new ArgumentOutOfRangeException(nameof(count), "Value cannot be negative.");
        }

        if (index + count > Count)
        {
            throw new ArgumentException($"Sum of values of '{nameof(index)}' and '{nameof(count)}' is out of bounds.");
        }

        switch (count)
        {
            case 0:
                return;
            case 1:
                RemoveItem(index);

                return;
        }

        BeforeReset?.Invoke(this, EventArgs.Empty);

        var previousShouldNotify = _shouldNotify;
        _shouldNotify = false;

        for (var i = index + count - 1; i >= index; i--)
        {
            RemoveItem(i);
        }

        _shouldNotify = previousShouldNotify;

        OnPropertyChanged(new PropertyChangedEventArgs("Count"));
        OnPropertyChanged(new PropertyChangedEventArgs("Item[]"));
        // Raising 'CollectionChanged' with ranges causes exceptions in the view.
        OnCollectionChanged(new NotifyCollectionChangedEventArgs(NotifyCollectionChangedAction.Reset));
    }

    /// <summary>
    /// Removes elements from the <see cref="BindableCollection{T}" /> based on the specified predicate.
    /// </summary>
    /// <param name="predicate">The predicate that defines the condition.</param>
    /// <returns>A number indicating the amount of removed elements.</returns>
    /// <exception cref="ArgumentNullException">Value of parameter <paramref name="predicate" /> is <c>null</c>.</exception>
    public int RemoveAll(Func<T, bool> predicate)
    {
        if (predicate is null)
        {
            throw new ArgumentNullException(nameof(predicate));
        }

        BeforeReset?.Invoke(this, EventArgs.Empty);

        var previousShouldNotify = _shouldNotify;
        _shouldNotify = false;

        var removeCount = 0;
        var startingIndex = int.MaxValue;

        for (var i = Count - 1; i >= 0; i--)
        {
            if (!predicate(this[i]))
            {
                continue;
            }

            RemoveItem(i);
            removeCount++;
            startingIndex = i;
        }

        _shouldNotify = previousShouldNotify;

        if (startingIndex == int.MaxValue)
        {
            NoReset?.Invoke(this, EventArgs.Empty);

            return removeCount;
        }

        OnPropertyChanged(new PropertyChangedEventArgs("Count"));
        OnPropertyChanged(new PropertyChangedEventArgs("Item[]"));
        // Raising 'CollectionChanged' with ranges causes exceptions in the view.
        OnCollectionChanged(new NotifyCollectionChangedEventArgs(NotifyCollectionChangedAction.Reset));

        return removeCount;
    }

    /// <inheritdoc />
    protected override void ClearItems()
    {
        BeforeReset?.Invoke(this, EventArgs.Empty);
        base.ClearItems();
    }

    /// <summary>
    /// Raises the <see cref="ObservableCollection{T}.CollectionChanged" /> event with the specified arguments.
    /// </summary>
    /// <param name="e">The arguments of the event.</param>
    protected override void OnCollectionChanged(NotifyCollectionChangedEventArgs e)
    {
        if (_shouldNotify)
        {
            base.OnCollectionChanged(e);
        }
    }

    /// <summary>
    /// Raises the <see cref="ObservableCollection{T}.PropertyChanged" /> event with the specified arguments.
    /// </summary>
    /// <param name="e">The arguments of the event.</param>
    protected override void OnPropertyChanged(PropertyChangedEventArgs e)
    {
        if (_shouldNotify)
        {
            base.OnPropertyChanged(e);
        }
    }
}
