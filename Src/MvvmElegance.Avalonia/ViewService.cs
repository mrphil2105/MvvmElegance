using Avalonia.Controls.ApplicationLifetimes;
using MvvmElegance.Internal;
using MvvmElegance.Xaml;

namespace MvvmElegance;

/// <summary>
/// Provides a service that allows view models to open windows and dialogs.
/// </summary>
public partial class ViewService : IViewService
{
    private readonly IExtendedViewManager _viewManager;

    /// <summary>
    /// Initializes a new instance of the <see cref="ViewService" /> class.
    /// </summary>
    /// <exception cref="InvalidOperationException">The application <see cref="BootstrapperBase" /> has not been initialized.</exception>
    public ViewService()
    {
        if (!Application.Current!.TryFindResource(View.ViewManagerResourceKey, out _viewManager!))
        {
            throw new InvalidOperationException(
                $"Instances of '{typeof(ViewService).FullName}' cannot be created before method " +
                $"'{typeof(BootstrapperBase).FullName}.{nameof(BootstrapperBase.Initialize)}' has been called.");
        }
    }

    /// <inheritdoc />
    /// <exception cref="ArgumentNullException">Value of parameter <paramref name="model" /> is <c>null</c>.</exception>
    public Task ShowAsync(object? ownerModel, object model)
    {
        if (model is null)
        {
            throw new ArgumentNullException(nameof(model));
        }

        return Dispatch.SendOrPostAsync(() =>
        {
            var (window, owner) = CreateWindow(model, false, ownerModel);

            var closedTcs = new TaskCompletionSource<object?>(TaskCreationOptions.RunContinuationsAsynchronously);
            window.Closed += (_, _) => closedTcs.TrySetResult(null);

            // 'Window.Show' unfortunately does not allow null owners even though it has a parameterless overload.
            if (owner != null)
            {
                window.Show(owner);
            }
            else
            {
                window.Show();
            }

            return closedTcs.Task;
        });
    }

    /// <inheritdoc />
    /// <exception cref="ArgumentNullException">Value of parameter <paramref name="model" /> is <c>null</c>.</exception>
    public Task<bool?> ShowDialogAsync(object? ownerModel, object model)
    {
        if (model is null)
        {
            throw new ArgumentNullException(nameof(model));
        }

        return Dispatch.SendOrPostAsync(() =>
        {
            var (window, owner) = CreateWindow(model, true, ownerModel);

            return window.ShowDialog<bool?>(owner);
        });
    }

    private (Window Window, Window? Owner) CreateWindow(object model, bool isDialog, object? ownerModel)
    {
        var view = _viewManager.CreateView(model);

        if (view is not Window window)
        {
            throw new InvalidOperationException("The associated view was of type " +
                $"'{view.GetType().FullName}', but must be of type '{typeof(Window).FullName}'.");
        }

        Window? owner = null;

        if (ownerModel != null)
        {
            owner = _viewManager.GetWindowConductor(ownerModel)
                .Window;
        }
        else if (isDialog)
        {
            owner = InferOwnerOf(window);
        }

        return (window, owner);
    }

    private Window? InferOwnerOf(Window? window)
    {
        var applicationLifetime = (IClassicDesktopStyleApplicationLifetime?)Application.Current?.ApplicationLifetime;

        if (applicationLifetime == null)
        {
            return null;
        }

        var activeWindow = applicationLifetime.Windows.FirstOrDefault(w => w.IsActive);
        activeWindow ??= applicationLifetime.MainWindow;

        return ReferenceEquals(activeWindow, window) ? null : activeWindow;
    }
}
