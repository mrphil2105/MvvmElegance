using System.Linq.Expressions;
using MvvmElegance.Xaml;

namespace MvvmElegance.Internal;

internal static class CommandActionHelper
{
    public const ActionUnavailableBehavior DefaultTargetNullBehavior = ActionUnavailableBehavior.Throw;
    public const ActionUnavailableBehavior DefaultMethodNotFoundBehavior = ActionUnavailableBehavior.Throw;

    public const string GuardNamePrefix = "Can";

    public static Func<object?, Task?>? CreateMethodFunc(object target, string methodName)
    {
        var targetType = target.GetType();
        var methodMethod = targetType.GetMethod(methodName);

        // We are ignoring a null value as it will be handled when attempting to invoke the target method.
        if (methodMethod == null)
        {
            return null;
        }

        if (methodMethod.ReturnType != typeof(void) && methodMethod.ReturnType != typeof(Task))
        {
            throw new ActionMethodInvalidException(
                $"Action method '{targetType.FullName}.{methodName}' must "
                    + $"have return type '{typeof(void).FullName}' or '{typeof(Task).FullName}'."
            );
        }

        var parameters = methodMethod.GetParameters();

        if (parameters.Length > 1)
        {
            throw new ActionMethodInvalidException(
                $"Action method '{targetType.FullName}.{methodName}' must have zero or one parameters."
            );
        }

        var parameter = parameters.SingleOrDefault();
        var returnType = methodMethod.ReturnType;

        var targetExpr = Expression.Constant(target);
        var argExpr = Expression.Parameter(typeof(object));
        var convertExpr = parameter != null ? Expression.Convert(argExpr, parameter.ParameterType) : null;
        var bodyExpr = (Expression)
            Expression.Call(targetExpr, methodMethod, convertExpr != null ? new Expression[] { convertExpr } : null);

        if (returnType == typeof(void))
        {
            var nullExpr = Expression.Constant(null, typeof(Task));
            bodyExpr = Expression.Block(bodyExpr, nullExpr);
        }

        return Expression.Lambda<Func<object?, Task?>>(bodyExpr, argExpr).Compile();
    }

    public static Func<bool>? CreateGuardFunc(object target, string guardName)
    {
        var targetType = target.GetType();
        var guardProperty = targetType.GetProperty(guardName);

        if (guardProperty == null)
        {
            return null;
        }

        if (guardProperty.PropertyType != typeof(bool))
        {
            throw new ActionGuardInvalidException(
                $"Action guard '{targetType.FullName}.{guardName}' must " + $"have type '{typeof(bool).FullName}'."
            );
        }

        if (guardProperty.GetGetMethod() == null)
        {
            throw new ActionGuardInvalidException(
                $"Action guard '{targetType.FullName}.{guardName}' must " + "have a public getter."
            );
        }

        var targetExpr = Expression.Constant(target);
        var propertyExpr = Expression.Property(targetExpr, guardProperty);

        return Expression.Lambda<Func<bool>>(propertyExpr).Compile();
    }
}
