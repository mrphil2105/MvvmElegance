using System.Diagnostics;
using System.Reflection;
using Avalonia.Metadata;
using MvvmElegance.Xaml;

namespace MvvmElegance.Internal;

internal class ExtendedViewManager : IExtendedViewManager
{
    private readonly ServiceFactory _serviceFactory;
    private readonly IViewTypeLocator _viewTypeLocator;

    private readonly Dictionary<object, WindowConductor> _modelWindowConductors;

    public ExtendedViewManager(ServiceFactory serviceFactory, IViewTypeLocator viewTypeLocator)
    {
        _serviceFactory = serviceFactory ?? throw new ArgumentNullException(nameof(serviceFactory));
        _viewTypeLocator = viewTypeLocator ?? throw new ArgumentNullException(nameof(viewTypeLocator));

        _modelWindowConductors = new Dictionary<object, WindowConductor>();
    }

    public Control CreateView(object model)
    {
        if (model is null)
        {
            throw new ArgumentNullException(nameof(model));
        }

        var modelType = model.GetType();
        var viewType = _viewTypeLocator.Locate(modelType);

        if (viewType.IsAbstract)
        {
            throw new ViewLocationException($"The view type '{viewType.FullName}' cannot be abstract.", modelType);
        }

        if (!typeof(Control).IsAssignableFrom(viewType))
        {
            throw new ViewLocationException(
                $"The view type '{viewType.FullName}' must derive from type '{typeof(Control).FullName}'.", modelType);
        }

        var view = (Control)_serviceFactory(viewType);

        View.SetActionTarget(view, model);
        view.DataContext = model;

        if (view is Window window)
        {
            var windowConductor = new WindowConductor(window, model);
            _modelWindowConductors.Add(model, windowConductor);

            window.Closed += OnWindowClosed;

            // TODO: Bind to 'Title' if model is 'IHaveDisplayName'.
        }
        else
        {
            // Delay the addition of the 'Window' for the model,
            // as we can only get the view's 'Window' after it has loaded.
            view.AttachedToVisualTree += OnAttachedToVisualTree;
            view.DetachedFromVisualTree += OnDetachedFromVisualTree;
        }

        return view;
    }

    public void SetModel(Control control, object? value)
    {
        if (control is null)
        {
            throw new ArgumentNullException(nameof(control));
        }

        if (value != null)
        {
            var view = CreateView(value);

            if (view is Window)
            {
                throw new InvalidOperationException(
                    $"The associated view must not be of type '{typeof(Window).FullName}'.");
            }

            SetContent(control, view);
        }
        else
        {
            SetContent(control, null);
        }
    }

    public WindowConductor GetWindowConductor(object model)
    {
        if (model is null)
        {
            throw new ArgumentNullException(nameof(model));
        }

        if (!_modelWindowConductors.TryGetValue(model, out var windowConductor))
        {
            throw new InvalidOperationException($"No owner window of type '{typeof(Window).FullName}' could be " +
                $"found for model of type '{model.GetType().FullName}'.");
        }

        return windowConductor;
    }

    // TODO: Support closing of 'TabControl' tabs too.
    public void CloseView(object model, bool? dialogResult = null)
    {
        if (model is null)
        {
            throw new ArgumentNullException(nameof(model));
        }

        if (!_modelWindowConductors.TryGetValue(model, out var windowConductor))
        {
            throw new InvalidOperationException($"No owner window of type '{typeof(Window).FullName}' could be " +
                $"found for model of type '{model.GetType().FullName}'.");
        }

        windowConductor.Close(dialogResult);
    }

    private static void SetContent(Control control, Control? view)
    {
        // TODO: Consider optimizing this by using System.Linq.Expressions.
        var controlType = control.GetType();

        // TODO: Is it guaranteed that all 'Content' properties have a 'ContentAttribute'?
        // If not we need to get the property by name as a fallback.
        var contentProperty = controlType.GetProperties()
            .SingleOrDefault(p => p.GetCustomAttribute<ContentAttribute>() != null);

        if (contentProperty == null)
        {
            throw new InvalidOperationException(
                $"Unable to find a content property on view type '{controlType.FullName}'.");
        }

        contentProperty.SetValue(control, view);
    }

    private void OnWindowClosed(object? sender, EventArgs e)
    {
        Debug.Assert(sender != null);

        var window = (Window)sender;
        window.Closed -= OnWindowClosed;

        var entriesToRemove = _modelWindowConductors.Where(kvp => ReferenceEquals(kvp.Value.Window, window))
            .ToList();

        foreach (var entry in entriesToRemove)
        {
            _modelWindowConductors.Remove(entry.Key);
        }
    }

    private void OnAttachedToVisualTree(object? sender, VisualTreeAttachmentEventArgs e)
    {
        Debug.Assert(sender != null);

        var element = (Control)sender;
        element.AttachedToVisualTree -= OnAttachedToVisualTree;

        var window = element.GetWindow();

        if (window == null)
        {
            return;
        }

        var windowConductor = _modelWindowConductors.Values.First(wc => ReferenceEquals(wc.Window, window));
        _modelWindowConductors.Add(element.DataContext!, windowConductor);
    }

    private void OnDetachedFromVisualTree(object? sender, VisualTreeAttachmentEventArgs e)
    {
        Debug.Assert(sender != null);

        var element = (Control)sender;
        element.DetachedFromVisualTree -= OnDetachedFromVisualTree;

        _modelWindowConductors.Remove(element.DataContext!);
    }
}
