using System.Collections.Specialized;

namespace MvvmElegance;

public partial class Conductor<T>
    where T : class
{
    public partial class Collection
    {
        public class AllActive : ConductorBase<T>
        {
            private List<T>? _itemsBeforeReset;

            public AllActive()
            {
                Items = new BindableCollection<T>();
                Items.CollectionChanged += OnCollectionChanged;
                Items.BeforeReset += OnBeforeReset;
                Items.NoReset += OnNoReset;
            }

            public BindableCollection<T> Items { get; }

            public override async Task ActivateItemAsync(T? item, CancellationToken cancellationToken = default)
            {
                if (item == null)
                {
                    return;
                }

                EnsureItem(item);

                if (IsActive)
                {
                    await ScreenExtensions.TryActivateAsync(item, cancellationToken);
                }
                else
                {
                    await ScreenExtensions.TryDeactivateAsync(item, cancellationToken);
                }
            }

            public override Task DeactivateItemAsync(T? item, CancellationToken cancellationToken = default)
            {
                return ScreenExtensions.TryDeactivateAsync(item, cancellationToken);
            }

            public override async Task CloseItemAsync(T? item, CancellationToken cancellationToken = default)
            {
                if (item == null || !await CanCloseItemAsync(item, cancellationToken))
                {
                    return;
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

            protected override Task OnActivateAsync(CancellationToken cancellationToken = default)
            {
                var tasks = Items.Select(i => ScreenExtensions.TryActivateAsync(i, cancellationToken))
                    .ToList();

                return Task.WhenAll(tasks);
            }

            protected override Task OnDeactivateAsync(CancellationToken cancellationToken = default)
            {
                var tasks = Items.Select(i => ScreenExtensions.TryDeactivateAsync(i, cancellationToken))
                    .ToList();

                return Task.WhenAll(tasks);
            }

            protected override Task OnCloseAsync(CancellationToken cancellationToken = default)
            {
                // Clearing 'Items' causes all items to be closed.
                Items.Clear();

                return Task.CompletedTask;
            }

            private async void OnCollectionChanged(object? sender, NotifyCollectionChangedEventArgs e)
            {
                switch (e.Action)
                {
                    case NotifyCollectionChangedAction.Add:
                        await this.SetParentAndSetActiveAsync(e.NewItems, IsActive);

                        break;
                    case NotifyCollectionChangedAction.Remove:
                        await this.TryCloseAndCleanUpAsync(e.OldItems, DisposeChildren);

                        break;
                    case NotifyCollectionChangedAction.Replace:
                        var oldItems = e.OldItems.Cast<T>()
                            .ToList();
                        var newItems = e.NewItems.Cast<T>()
                            .ToList();

                        // Items may be replaced with themselves, so prevent a close/cleanup followed by a reactivation.
                        await this.TryCloseAndCleanUpAsync(oldItems.Except(newItems), DisposeChildren);
                        await this.SetParentAndSetActiveAsync(newItems.Except(oldItems), IsActive);

                        break;
                    case NotifyCollectionChangedAction.Reset:
                        await this.TryCloseAndCleanUpAsync(_itemsBeforeReset.Except(Items), DisposeChildren);
                        await this.SetParentAndSetActiveAsync(Items.Except(_itemsBeforeReset), IsActive);
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
