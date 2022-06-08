using System.Collections.ObjectModel;
using System.Collections.Specialized;
using System.ComponentModel;

namespace MvvmElegance;

public class BindableCollection<T> : ObservableCollection<T>
{
    private bool _shouldNotify;

    public BindableCollection()
    {
        _shouldNotify = true;
    }

    public BindableCollection(IEnumerable<T> items) : base(items)
    {
        _shouldNotify = true;
    }

    internal event EventHandler? BeforeReset;

    public void AddRange(IEnumerable<T> items)
    {
        InsertRange(Count, items);
    }

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

        bool previousShouldNotify = _shouldNotify;
        _shouldNotify = false;

        int startingIndex = index;

        foreach (var item in items)
        {
            InsertItem(index, item);
            index++;
        }

        _shouldNotify = previousShouldNotify;

        if (index == startingIndex)
        {
            return;
        }

        OnPropertyChanged(new PropertyChangedEventArgs("Count"));
        OnPropertyChanged(new PropertyChangedEventArgs("Item[]"));
        // Raising 'CollectionChanged' with ranges causes exceptions in the view.
        OnCollectionChanged(new NotifyCollectionChangedEventArgs(NotifyCollectionChangedAction.Reset));
    }

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

        bool previousShouldNotify = _shouldNotify;
        _shouldNotify = false;

        for (int i = index + count - 1; i >= index; i--)
        {
            RemoveItem(i);
        }

        _shouldNotify = previousShouldNotify;

        OnPropertyChanged(new PropertyChangedEventArgs("Count"));
        OnPropertyChanged(new PropertyChangedEventArgs("Item[]"));
        // Raising 'CollectionChanged' with ranges causes exceptions in the view.
        OnCollectionChanged(new NotifyCollectionChangedEventArgs(NotifyCollectionChangedAction.Reset));
    }

    public int RemoveAll(Func<T, bool> predicate)
    {
        if (predicate is null)
        {
            throw new ArgumentNullException(nameof(predicate));
        }

        bool previousShouldNotify = _shouldNotify;
        _shouldNotify = false;

        int removeCount = 0;
        int startingIndex = int.MaxValue;

        for (int i = Count - 1; i >= 0; i--)
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
            return removeCount;
        }

        OnPropertyChanged(new PropertyChangedEventArgs("Count"));
        OnPropertyChanged(new PropertyChangedEventArgs("Item[]"));
        // Raising 'CollectionChanged' with ranges causes exceptions in the view.
        OnCollectionChanged(new NotifyCollectionChangedEventArgs(NotifyCollectionChangedAction.Reset));

        return removeCount;
    }

    protected override void ClearItems()
    {
        BeforeReset?.Invoke(this, EventArgs.Empty);
        base.ClearItems();
    }

    protected override void OnCollectionChanged(NotifyCollectionChangedEventArgs e)
    {
        if (_shouldNotify)
        {
            base.OnCollectionChanged(e);
        }
    }

    protected override void OnPropertyChanged(PropertyChangedEventArgs e)
    {
        if (_shouldNotify)
        {
            base.OnPropertyChanged(e);
        }
    }
}
