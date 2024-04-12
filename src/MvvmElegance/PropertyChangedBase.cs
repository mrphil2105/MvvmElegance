using System.ComponentModel;
using System.Linq.Expressions;
using System.Reflection;
using System.Runtime.CompilerServices;

namespace MvvmElegance;

/// <summary>
/// Provides the base class for observable models.
/// </summary>
public abstract class PropertyChangedBase : INotifyPropertyChanged
{
    /// <summary>
    /// An event that is raised when a property has changed.
    /// </summary>
    public event PropertyChangedEventHandler? PropertyChanged;

    /// <summary>
    /// Sets the specified field to the specified value and raises the <see cref="PropertyChanged" /> event if the value has changed.
    /// </summary>
    /// <param name="target">The field to assign the specified value to.</param>
    /// <param name="value">The value to assign to the specified field.</param>
    /// <param name="propertyName">The name of the property associated with the specified field.</param>
    /// <typeparam name="T">The type of the specified field and value.</typeparam>
    /// <returns>A boolean indicating whether the value has changed.</returns>
    protected virtual bool Set<T>(ref T target, T value, [CallerMemberName] string? propertyName = null)
    {
        if (EqualityComparer<T>.Default.Equals(target, value))
        {
            return false;
        }

        target = value;
        RaisePropertyChanged(propertyName);

        return true;
    }

    /// <summary>
    /// Sets the member of the specified expression to the specified value and raises the <see cref="PropertyChanged" /> event if the value has changed.
    /// </summary>
    /// <param name="targetExpr">The expression containing the member to assign the specified value to.</param>
    /// <param name="value">The value to assign to the specified member.</param>
    /// <param name="propertyName">The name of the property associated with the specified member.</param>
    /// <typeparam name="T">The type of the specified member and value.</typeparam>
    /// <returns>A boolean indicating whether the value has changed.</returns>
    /// <exception cref="ArgumentNullException">Value of parameter <paramref name="targetExpr" /> is <c>null</c>.</exception>
    /// <exception cref="ArgumentException">Value of parameter <paramref name="targetExpr" /> does not contain a body that is a member expression.</exception>
    protected virtual bool Set<T>(
        Expression<Func<T>> targetExpr,
        T value,
        [CallerMemberName] string? propertyName = null
    )
    {
        if (targetExpr is null)
        {
            throw new ArgumentNullException(nameof(targetExpr));
        }

        PropertyInfo? targetProperty;
        FieldInfo? targetField = null;

        if (
            targetExpr.Body is not MemberExpression memberExpr
            || (
                (targetProperty = memberExpr.Member as PropertyInfo) is null
                && (targetField = memberExpr.Member as FieldInfo) is null
            )
        )
        {
            throw new ArgumentException("Value must contain a body that is a member expression.", nameof(targetExpr));
        }

        if (!TryGetMemberOwner(memberExpr, out var owner))
        {
            throw new ArgumentException("Value must contain a body that is a member expression.", nameof(targetExpr));
        }

        if (owner == null)
        {
            throw new ArgumentException("Value cannot contain a model that is null.", nameof(targetExpr));
        }

        var oldValue = (T)(targetProperty is null ? targetField!.GetValue(owner) : targetProperty.GetValue(owner));

        if (EqualityComparer<T>.Default.Equals(oldValue, value))
        {
            return false;
        }

        if (targetProperty is not null)
        {
            targetProperty.SetValue(owner, value);
        }
        else
        {
            targetField!.SetValue(owner, value);
        }

        RaisePropertyChanged(propertyName);

        return true;
    }

    /// <summary>
    /// Raises the <see cref="PropertyChanged" /> event with the specified property name.
    /// </summary>
    /// <param name="propertyName">The name of the changed property.</param>
    protected virtual void RaisePropertyChanged(string? propertyName)
    {
        OnPropertyChanged(new PropertyChangedEventArgs(propertyName));
    }

    /// <summary>
    /// Raises the <see cref="PropertyChanged" /> event with the name of the property of the specified expression.
    /// </summary>
    /// <param name="propertyExpr">The expression containing the changed property.</param>
    /// <typeparam name="TProperty">The type of the specified property.</typeparam>
    protected virtual void RaisePropertyChanged<TProperty>(Expression<Func<TProperty>> propertyExpr)
    {
        RaisePropertyChanged(propertyExpr.GetPropertyName());
    }

    /// <summary>
    /// Raises the <see cref="PropertyChanged" /> event with the specified arguments.
    /// </summary>
    /// <param name="e">The arguments of the event.</param>
    protected virtual void OnPropertyChanged(PropertyChangedEventArgs e)
    {
        PropertyChanged?.Invoke(this, e);
    }

    private static bool TryGetMemberOwner(MemberExpression memberExpr, out object? owner)
    {
        switch (memberExpr.Expression)
        {
            // The owner of the property or field is the current instance.
            case ConstantExpression ownerExpr:
                owner = ownerExpr.Value;

                return owner != null;
            // The current instance wraps a model which is the owner of the property or field.
            case MemberExpression { Expression: ConstantExpression obsModelExpr } modelExpr:
                if (obsModelExpr.Value == null)
                {
                    owner = null;

                    return false;
                }

                owner = modelExpr.Member switch
                {
                    PropertyInfo property => property.GetValue(obsModelExpr.Value),
                    FieldInfo field => field.GetValue(obsModelExpr.Value),
                    _ => null
                };

                return true;
            default:
                owner = null;

                return false;
        }
    }
}
