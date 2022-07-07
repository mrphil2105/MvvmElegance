using System.Collections.Specialized;
using System.Diagnostics;

namespace MvvmElegance;

public partial class Conductor<T>
    where T : class
{
    /// <summary>
    /// Provides the parent class for all collection conductors.
    /// </summary>
    public partial class Collection
    {
        /// <summary>
        /// Provides a collection conductor with one active item.
        /// </summary>
        public class OneActive : ConductorBaseWithActiveItem<T>
        {
            private List<T>? _itemsBeforeReset;
            private int _removedIndex;

            /// <summary>
            /// Initializes a new instance of the <see cref="OneActive" /> class.
            /// </summary>
            public OneActive()
            {
                Items = new BindableCollection<T>();
                Items.CollectionChanged += OnCollectionChanged;
                Items.BeforeReset += OnBeforeReset;
                Items.NoReset += OnNoReset;
            }

            /// <summary>
            /// Gets the items as an observable collection.
            /// </summary>
            public BindableCollection<T> Items { get; }

            /// <summary>
            /// Activates the specified item, sets it as the active item and adds it to the <see cref="Items" /> list, if applicable.
            /// </summary>
            /// <param name="item">The item to activate and set as active item.</param>
            /// <param name="cancellationToken">The cancellation token used to cancel the operation.</param>
            /// <returns>A task representing the asynchronous operation.</returns>
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

            /// <summary>
            /// Deactivates the specified item and removes it as the active item, if applicable.
            /// </summary>
            /// <param name="item">The item to deactivate.</param>
            /// <param name="cancellationToken">The cancellation token used to cancel the operation.</param>
            /// <returns>A task representing the asynchronous operation.</returns>
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

            /// <summary>
            /// Closes the specified item and removes it as the active item and from the <see cref="Items" /> list, if applicable.
            /// </summary>
            /// <param name="item">The item to close.</param>
            /// <param name="cancellationToken">The cancellation token used to cancel the operation.</param>
            /// <returns>A task representing the asynchronous operation.</returns>
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

            /// <inheritdoc />
            public override IEnumerable<T> GetChildren()
            {
                return Items;
            }

            /// <inheritdoc />
            public override Task<bool> CanCloseAsync(CancellationToken cancellationToken = default)
            {
                return CanCloseAllItemsAsync(Items, cancellationToken);
            }

            /// <summary>
            /// Ensures the specified item is ready for activation/deactivation by adding it to the <see cref="Items" /> list and setting the parent.
            /// </summary>
            /// <param name="item">The item to ensure.</param>
            protected override void EnsureItem(T? item)
            {
                if (item != null && !Items.Contains(item))
                {
                    Items.Add(item);
                }

                base.EnsureItem(item);
            }

            /// <inheritdoc />
            protected override Task OnCloseAsync(CancellationToken cancellationToken = default)
            {
                // Clearing 'Items' causes all items to be closed.
                Items.Clear();

                return Task.CompletedTask;
            }

            /// <summary>
            /// Gets the next item to activate given the specified previous active item.
            /// </summary>
            /// <param name="item">The previous active item.</param>
            /// <returns>An item to set as the active item.</returns>
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

            /// <summary>
            /// Called when the active item may need replacing when the <see cref="Items" /> list has been modified.
            /// </summary>
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

                        var oldItems = e.OldItems.Cast<T>()
                            .ToList();
                        var newItems = e.NewItems.Cast<T>()
                            .ToList();

                        await ReplaceActiveItemIfNeededAsync();
                        // Items may be replaced with themselves, so prevent a close/cleanup followed by a reactivation.
                        await this.TryCloseAndCleanUpAsync(oldItems.Except(newItems), DisposeChildren);
                        await this.SetParentAndSetActiveAsync(newItems.Except(oldItems), false);

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
