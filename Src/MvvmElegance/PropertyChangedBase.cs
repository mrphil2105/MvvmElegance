using System.ComponentModel;
using System.Linq.Expressions;
using System.Reflection;
using System.Runtime.CompilerServices;

namespace MvvmElegance;

public abstract class PropertyChangedBase : INotifyPropertyChanged
{
    public event PropertyChangedEventHandler? PropertyChanged;

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

    protected virtual bool Set<T>(Expression<Func<T>> targetExpr, T value,
        [CallerMemberName] string? propertyName = null)
    {
        if (targetExpr is null)
        {
            throw new ArgumentNullException(nameof(targetExpr));
        }

        PropertyInfo? targetProperty;
        FieldInfo? targetField = null;

        if (targetExpr.Body is not MemberExpression memberExpr ||
            (targetProperty = memberExpr.Member as PropertyInfo) is null &&
            (targetField = memberExpr.Member as FieldInfo) is null)
        {
            throw new ArgumentException("Value must contain a body that is a member expression.", nameof(targetExpr));
        }

        if (!TryGetMemberOwner(memberExpr, out object? owner))
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

    protected virtual void RaisePropertyChanged(string? propertyName)
    {
        OnPropertyChanged(new PropertyChangedEventArgs(propertyName));
    }

    protected virtual void RaisePropertyChanged<TProperty>(Expression<Func<TProperty>> propertyExpr)
    {
        RaisePropertyChanged(propertyExpr.GetPropertyName());
    }

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
