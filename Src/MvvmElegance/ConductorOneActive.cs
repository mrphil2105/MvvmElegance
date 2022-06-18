using System.Collections.Specialized;
using System.Diagnostics;

namespace MvvmElegance;

public partial class Conductor<T>
    where T : class
{
    public partial class Collection
    {
        public class OneActive : ConductorBaseWithActiveItem<T>
        {
            private List<T>? _itemsBeforeReset;
            private int _removedIndex;

            public OneActive()
            {
                Items = new BindableCollection<T>();
                Items.CollectionChanged += OnCollectionChanged;
                Items.BeforeReset += OnBeforeReset;
                Items.NoReset += OnNoReset;
            }

            public BindableCollection<T> Items { get; }

            public override Task ActivateItemAsync(T? item, CancellationToken cancellationToken = default)
            {
                if (item != null && EqualityComparer<T?>.Default.Equals(item, ActiveItem))
                {
                    return IsActive
                        ? ScreenExtensions.TryActivateAsync(ActiveItem, cancellationToken)
                        : ScreenExtensions.TryDeactivateAsync(ActiveItem, cancellationToken);
                }

                return ChangeActiveItemAsync(item, false, cancellationToken);
            }

            public override Task DeactivateItemAsync(T? item, CancellationToken cancellationToken = default)
            {
                if (item == null)
                {
                    return Task.CompletedTask;
                }

                if (!EqualityComparer<T?>.Default.Equals(item, ActiveItem))
                {
                    return ScreenExtensions.TryDeactivateAsync(item, cancellationToken);
                }

                var nextItem = GetNextItemToActivate(item);

                return ChangeActiveItemAsync(nextItem, false, cancellationToken);
            }

            public override async Task CloseItemAsync(T? item, CancellationToken cancellationToken = default)
            {
                if (item == null || !await CanCloseItemAsync(item, cancellationToken))
                {
                    return;
                }

                if (EqualityComparer<T?>.Default.Equals(item, ActiveItem))
                {
                    var nextItem = GetNextItemToActivate(item);

                    await ChangeActiveItemAsync(nextItem, false, cancellationToken);
                }

                // Successfully removing the item from 'Items' will close it.
                if (!Items.Remove(item))
                {
                    await this.TryCloseAndCleanUpAsync(item, DisposeChildren, cancellationToken);
                }
            }

            public override IEnumerable<T> GetChildren()
            {
                return Items;
            }

            public override Task<bool> CanCloseAsync(CancellationToken cancellationToken = default)
            {
                return CanCloseAllItemsAsync(Items, cancellationToken);
            }

            protected override void EnsureItem(T? item)
            {
                if (item != null && !Items.Contains(item))
                {
                    Items.Add(item);
                }

                base.EnsureItem(item);
            }

            protected override Task OnCloseAsync(CancellationToken cancellationToken = default)
            {
                // Clearing 'Items' causes all items to be closed.
                Items.Clear();

                return Task.CompletedTask;
            }

            protected virtual T? GetNextItemToActivate(T? item)
            {
                // We cannot simply use 'Items.Count <= 1' here instead of 'Items.Count == 0' and 'Items.Count == 1',
                // because we do not want to return null when 'Items' only has one element that is NOT the item.
                // In that case we want to return the first item of 'Items' as the next item to activate.

                if (Items.Count == 0)
                {
                    return null;
                }

                int index;

                if (item == null || (index = Items.IndexOf(item)) == -1)
                {
                    return Items[0];
                }

                if (Items.Count == 1)
                {
                    return null;
                }

                return index == 0 ? Items[0] : Items[index - 1];
            }

            protected virtual async Task ReplaceActiveItemIfNeededAsync()
            {
                // The active item may have been removed.
                if (Items.Contains(ActiveItem!))
                {
                    return;
                }

                T? nextItem;

                if (Items.Count > 0)
                {
                    var nextItemIndex = Math.Max(_removedIndex - 1, 0);
                    nextItem = Items[nextItemIndex];
                }
                else
                {
                    nextItem = null;
                }

                await ChangeActiveItemAsync(nextItem, false);
            }

            private async void OnCollectionChanged(object? sender, NotifyCollectionChangedEventArgs e)
            {
                switch (e.Action)
                {
                    case NotifyCollectionChangedAction.Add:
                        await this.SetParentAndSetActiveAsync(e.NewItems, false);

                        break;
                    case NotifyCollectionChangedAction.Remove:
                        _removedIndex = e.OldStartingIndex;

                        await ReplaceActiveItemIfNeededAsync();
                        await this.TryCloseAndCleanUpAsync(e.OldItems, DisposeChildren);

                        break;
                    case NotifyCollectionChangedAction.Replace:
                        _removedIndex = e.OldStartingIndex;

                        await ReplaceActiveItemIfNeededAsync();
                        // Items may be replaced with themselves, which causes a close/cleanup followed by a reactivation.
                        await this.TryCloseAndCleanUpAsync(e.OldItems, DisposeChildren);
                        await this.SetParentAndSetActiveAsync(e.NewItems, false);

                        break;
                    case NotifyCollectionChangedAction.Reset:
                        Debug.Assert(_itemsBeforeReset != null);
                        // Unfortunately 'Reset' has to be used for range operations on 'BindableCollection'.
                        // This means we do not know from where in 'Items' e.g. a range removal was made.
                        // On range removal, we cannot activate the item before the removed items, so we activate the first item.
                        // Instruct 'ReplaceActiveItemIfNeededAsync' to activate the first item by setting '_removedIndex' to '-1'.
                        _removedIndex = -1;

                        await ReplaceActiveItemIfNeededAsync();
                        await this.TryCloseAndCleanUpAsync(_itemsBeforeReset.Except(Items), DisposeChildren);
                        await this.SetParentAndSetActiveAsync(Items.Except(_itemsBeforeReset), false);
                        _itemsBeforeReset = null;

                        break;
                }
            }

            private void OnBeforeReset(object? sender, EventArgs e)
            {
                _itemsBeforeReset = Items.ToList();
            }

            private void OnNoReset(object? sender, EventArgs e)
            {
                // Set the list to null to prevent unintentionally holding on to unwanted references.
                _itemsBeforeReset = null;
            }
        }
    }
}
