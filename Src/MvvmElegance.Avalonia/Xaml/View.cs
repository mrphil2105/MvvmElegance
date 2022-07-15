using MvvmElegance.Internal;

namespace MvvmElegance.Xaml;

/// <summary>
/// Provides attached properties relating to various parts of the view.
/// </summary>
public static class View
{
    /// <summary>
    /// Key used to retrieve the view manager from the application resources.
    /// </summary>
    public const string ViewManagerResourceKey = "616a1a03-5622-4cb7-bc41-6cfc5c42147e";

    static View()
    {
        ModelProperty.Changed.Subscribe(OnModelChanged);
        ActionTargetProperty.Changed.Subscribe(_ => { });
    }

    /// <summary>
    /// An attached property specifying the model associated with a given control.
    /// </summary>
    public static readonly AttachedProperty<object> ModelProperty =
        AvaloniaProperty.RegisterAttached<ContentControl, Control, object>("Model");

    /// <summary>
    /// Gets the model associated with the specified control.
    /// </summary>
    /// <param name="element">The control to get the model for.</param>
    /// <returns>The model associated with the specified control.</returns>
    public static object GetModel(Control element)
    {
        return element.GetValue(ModelProperty);
    }

    /// <summary>
    /// Sets the model associated with the specified control.
    /// </summary>
    /// <param name="element">The control to set the model for.</param>
    /// <param name="value">The model to set for the specified control.</param>
    public static void SetModel(Control element, object value)
    {
        element.SetValue(ModelProperty, value);
    }

    private static void OnModelChanged(AvaloniaPropertyChangedEventArgs e)
    {
        if (e.Sender is not Control element)
        {
            throw new InvalidOperationException(
                $"Value of property '{typeof(AvaloniaPropertyChangedEventArgs).FullName}." +
                $"{nameof(AvaloniaPropertyChangedEventArgs.Sender)}' must be of type " +
                $"'{typeof(Control).FullName}'.");
        }

        if (!Application.Current!.TryFindResource<IExtendedViewManager>(ViewManagerResourceKey, out var viewManager))
        {
            throw new InvalidOperationException("Value of attached property " +
                $"'{typeof(View).FullName}.{nameof(ModelProperty)}' cannot be set before method " +
                $"'{typeof(BootstrapperBase).FullName}.{nameof(BootstrapperBase.Initialize)}' has been called.");
        }

        viewManager!.SetModel(element, e.NewValue);
    }

    /// <summary>
    /// An attached property specifying the action target associated with a given control for actions.
    /// </summary>
    public static readonly AttachedProperty<object?> ActionTargetProperty =
        AvaloniaProperty.RegisterAttached<ContentControl, Control, object?>("ActionTarget", null, true);

    /// <summary>
    /// Gets the action target associated with the specified control.
    /// </summary>
    /// <param name="element">The control to get the action target for.</param>
    /// <returns>The action target associated with the specified control.</returns>
    public static object? GetActionTarget(Control element)
    {
        return element.GetValue(ActionTargetProperty);
    }

    /// <summary>
    /// Sets the action target associated with the specified control.
    /// </summary>
    /// <param name="element">The control to set the action target for.</param>
    /// <param name="value">The action target to set for the specified control.</param>
    public static void SetActionTarget(Control element, object? value)
    {
        element.SetValue(ActionTargetProperty, value);
    }
}
