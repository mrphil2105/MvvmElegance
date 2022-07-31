using System.Windows.Input;
using Avalonia.Markup.Xaml;

namespace MvvmElegance.Xaml;

/// <summary>
/// Provides a markup extension used for binding commands to methods on the action target.
/// </summary>
public class ActionExtension : MarkupExtension
{
    /// <summary>
    /// Initializes a new instance of the <see cref="ActionExtension" /> class.
    /// </summary>
    public ActionExtension()
    {
    }

    /// <summary>
    /// Initializes a new instance of the <see cref="ActionExtension" /> class with the specified action method name.
    /// </summary>
    /// <param name="methodName">The name of the action method to call.</param>
    public ActionExtension(string methodName)
    {
        MethodName = methodName;
    }

    /// <summary>
    /// Gets or sets the name of the action method to call.
    /// </summary>
    [ConstructorArgument("methodName")]
    public string? MethodName { get; set; }

    /// <summary>
    /// Gets or sets the desired behavior when the action target is <c>null</c>.
    /// </summary>
    public ActionUnavailableBehavior TargetNullBehavior { get; set; }

    /// <summary>
    /// Gets or sets the desired behavior when the action method could not be found.
    /// </summary>
    public ActionUnavailableBehavior MethodNotFoundBehavior { get; set; }

    /// <inheritdoc />
    /// <exception cref="InvalidOperationException">The markup extension is used on an invalid object or a property that
    /// is not of type <see cref="ICommand" />.</exception>
    public override object ProvideValue(IServiceProvider serviceProvider)
    {
        if (MethodName == null)
        {
            throw new InvalidOperationException(
                $"Value of property '{typeof(ActionExtension).FullName}.{nameof(MethodName)}' cannot be null.");
        }

        var valueService = (IProvideValueTarget)serviceProvider.GetService(typeof(IProvideValueTarget));

        if (valueService.TargetObject is not AvaloniaObject targetObject)
        {
            throw new InvalidOperationException(
                $"Markup extension of type '{typeof(ActionExtension).FullName}' can only be used with objects of " +
                $"type '{typeof(AvaloniaObject).FullName}'.");
        }

        if (valueService.TargetProperty is AvaloniaProperty targetProperty &&
            targetProperty.PropertyType == typeof(ICommand))
        {
            return new CommandAction(targetObject, MethodName, TargetNullBehavior, MethodNotFoundBehavior);
        }

        throw new InvalidOperationException(
            $"Markup extension of type '{typeof(ActionExtension).FullName}' can only be used with properties of " +
            $"type '{typeof(ICommand).FullName}'.");
    }
}
