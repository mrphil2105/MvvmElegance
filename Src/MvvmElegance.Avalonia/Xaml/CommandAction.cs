using System.ComponentModel;
using System.Windows.Input;
using MvvmElegance.Internal;

namespace MvvmElegance.Xaml;

/// <summary>
/// Provides an <see cref="ICommand" /> for binding controls to void-returning or Task-returning methods on a view model.
/// </summary>
public class CommandAction : AvaloniaObject, ICommand
{
    /// <summary>
    /// A direct property specifying the action target.
    /// </summary>
    public static readonly DirectProperty<CommandAction, object?> TargetProperty =
        AvaloniaProperty.RegisterDirect<CommandAction, object?>(nameof(Target), o => o.Target, (a, t) => a.Target = t);

    private object? _target;

    /// <summary>
    /// Gets or sets the action target.
    /// </summary>
    public object? Target
    {
        get => _target;
        set => SetAndRaise(TargetProperty, ref _target, value);
    }

    private Func<object?, Task?>? _methodFunc;
    private Func<bool>? _guardFunc;

    private bool _isExecuting;

    /// <summary>
    /// Initializes a new instance of the <see cref="CommandAction" /> class with the specified element, method name, target null behavior and
    /// method not found behavior.
    /// </summary>
    /// <param name="element">The element that the action is defined on.</param>
    /// <param name="methodName">The name of the action method to call.</param>
    /// <param name="targetNullBehavior">The desired behavior when the action target is <c>null</c>.</param>
    /// <param name="methodNotFoundBehavior">The desired behavior when the action method could not be found.</param>
    /// <exception cref="ArgumentOutOfRangeException">Value of parameter <paramref name="targetNullBehavior" /> or <paramref name="methodNotFoundBehavior" />
    /// is not defined in the enumeration.</exception>
    /// <exception cref="ArgumentNullException">Value of parameter <paramref name="element" /> or <paramref name="methodName" /> is <c>null</c>.</exception>
    public CommandAction(AvaloniaObject element, string methodName, ActionUnavailableBehavior targetNullBehavior,
        ActionUnavailableBehavior methodNotFoundBehavior)
    {
        if (!Enum.IsDefined(typeof(ActionUnavailableBehavior), targetNullBehavior))
        {
            throw new ArgumentOutOfRangeException(nameof(targetNullBehavior),
                $"Value must be defined in the '{typeof(ActionUnavailableBehavior).FullName}' enumeration.");
        }

        if (!Enum.IsDefined(typeof(ActionUnavailableBehavior), methodNotFoundBehavior))
        {
            throw new ArgumentOutOfRangeException(nameof(methodNotFoundBehavior),
                $"Value must be defined in the '{typeof(ActionUnavailableBehavior).FullName}' enumeration.");
        }

        Element = element ?? throw new ArgumentNullException(nameof(element));
        MethodName = methodName ?? throw new ArgumentNullException(nameof(methodName));

        TargetNullBehavior = targetNullBehavior == ActionUnavailableBehavior.Default
            ? CommandActionHelper.DefaultTargetNullBehavior
            : targetNullBehavior;
        MethodNotFoundBehavior = methodNotFoundBehavior == ActionUnavailableBehavior.Default
            ? CommandActionHelper.DefaultMethodNotFoundBehavior
            : methodNotFoundBehavior;

        this[!TargetProperty] = Element[!View.ActionTargetProperty];
    }

    /// <inheritdoc />
    public event EventHandler? CanExecuteChanged;

    /// <summary>
    /// Gets the element that the action is defined on.
    /// </summary>
    public AvaloniaObject Element { get; }

    /// <summary>
    /// Gets the name of the action method to call.
    /// </summary>
    public string MethodName { get; }

    /// <summary>
    /// Gets the desired behavior when the action target is <c>null</c>.
    /// </summary>
    public ActionUnavailableBehavior TargetNullBehavior { get; }

    /// <summary>
    /// Gets the desired behavior when the action method could not be found.
    /// </summary>
    public ActionUnavailableBehavior MethodNotFoundBehavior { get; }

    /// <inheritdoc />
    public bool CanExecute(object? obj)
    {
        if (Target == null)
        {
            return TargetNullBehavior != ActionUnavailableBehavior.Disable;
        }

        if (_methodFunc == null)
        {
            return MethodNotFoundBehavior != ActionUnavailableBehavior.Disable;
        }

        return !_isExecuting && (_guardFunc?.Invoke() ?? true);
    }

    /// <inheritdoc />
    /// <exception cref="ActionMethodNotFoundException">The action method could not be found.</exception>
    /// <exception cref="ActionTargetNullException">The action target is <c>null</c>.</exception>
    public async void Execute(object? obj)
    {
        if (Target != null)
        {
            // We place this check here as '_methodFunc' will always be null if 'Target' is.
            if (_methodFunc != null)
            {
                var task = _methodFunc(obj);

                if (task == null)
                {
                    return;
                }

                _isExecuting = true;
                CanExecuteChanged?.Invoke(this, EventArgs.Empty);

                try
                {
                    await task;
                }
                finally
                {
                    _isExecuting = false;
                    CanExecuteChanged?.Invoke(this, EventArgs.Empty);
                }
            }
            else if (MethodNotFoundBehavior == ActionUnavailableBehavior.Throw)
            {
                throw new ActionMethodNotFoundException(
                    $"Action method '{Target.GetType().FullName}.{MethodName}' could not be found.");
            }
        }
        else if (TargetNullBehavior == ActionUnavailableBehavior.Throw)
        {
            throw new ActionTargetNullException($"Value of attached property '{typeof(View).FullName}." +
                $"{nameof(View.ActionTargetProperty)}' on element of type '{Element.GetType().FullName}' " +
                $"cannot be null (action method name: '{MethodName}').");
        }
    }

    /// <inheritdoc />
    protected override void OnPropertyChanged<T>(AvaloniaPropertyChangedEventArgs<T> e)
    {
        if (e.Property != TargetProperty)
        {
            return;
        }

        var newValue = e.NewValue.Value;
        var oldValue = e.OldValue.Value;

        var action = (CommandAction)e.Sender;
        action.UpdateMethodFunc(newValue);
        action.OnTargetChanged(oldValue, newValue);
    }

    private void UpdateMethodFunc(object? target)
    {
        _methodFunc = null;

        // We are ignoring a null value as it will be handled when attempting to invoke the target method.
        if (target != null)
        {
            _methodFunc = CommandActionHelper.CreateMethodFunc(target, MethodName);
        }
    }

    private void OnTargetChanged(object? oldTarget, object? newTarget)
    {
        var guardName = CommandActionHelper.GuardNamePrefix + MethodName;

        if (oldTarget is INotifyPropertyChanged oldNpc)
        {
            oldNpc.PropertyChanged -= OnPropertyChanged;
        }

        _guardFunc = null;

        if (newTarget is INotifyPropertyChanged newNpc)
        {
            _guardFunc = CommandActionHelper.CreateGuardFunc(newTarget, guardName);

            if (_guardFunc != null)
            {
                newNpc.PropertyChanged += OnPropertyChanged;
            }
        }

        CanExecuteChanged?.Invoke(this, EventArgs.Empty);
    }

    private void OnPropertyChanged(object? sender, PropertyChangedEventArgs e)
    {
        CanExecuteChanged?.Invoke(this, EventArgs.Empty);
    }
}
