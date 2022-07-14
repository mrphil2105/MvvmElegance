using System.ComponentModel;

namespace MvvmElegance.Internal;

internal class WindowConductor
{
    private readonly object _model;

    private bool? _targetDialogResult;
    private bool _isActuallyClosing;

    public WindowConductor(Window window, object model)
    {
        Window = window ?? throw new ArgumentNullException(nameof(window));
        _model = model ?? throw new ArgumentNullException(nameof(model));

        if (_model is IScreenState)
        {
            Window.PropertyChanged += OnPropertyChanged;
        }

        if (_model is IGuardClose)
        {
            Window.Closing += OnClosing;
        }

        Window.Closed += OnClosed;
    }

    public Window Window { get; }

    public void Close(bool? dialogResult)
    {
        _targetDialogResult = dialogResult;
        Window.Close(dialogResult);
    }

    private async void OnPropertyChanged(object? sender, AvaloniaPropertyChangedEventArgs e)
    {
        if (e.Property == Window.WindowStateProperty)
        {
            switch (Window.WindowState)
            {
                case WindowState.Normal:
                case WindowState.Maximized:
                    await ScreenExtensions.TryActivateAsync(_model);

                    break;
                case WindowState.Minimized:
                    await ScreenExtensions.TryDeactivateAsync(_model);

                    break;
            }
        }
        else if (e.Property == Visual.IsVisibleProperty)
        {
            if (Window.IsVisible)
            {
                await ScreenExtensions.TryActivateAsync(_model);
            }
            else
            {
                await ScreenExtensions.TryDeactivateAsync(_model);
            }
        }
    }

    private async void OnClosing(object? sender, CancelEventArgs e)
    {
        if (e.Cancel)
        {
            return;
        }

        if (_isActuallyClosing)
        {
            _isActuallyClosing = false;

            return;
        }

        e.Cancel = true;

        var canClose = await ((IGuardClose)_model).CanCloseAsync();

        if (!canClose)
        {
            return;
        }

        _isActuallyClosing = true;
        Window.Close(_targetDialogResult);
    }

    private async void OnClosed(object? sender, EventArgs e)
    {
        Window.PropertyChanged -= OnPropertyChanged;
        Window.Closing -= OnClosing;
        Window.Closed -= OnClosed;

        await ScreenExtensions.TryCloseAsync(_model);
    }
}
