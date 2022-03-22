using System.Linq.Expressions;

namespace MvvmElegance;

public static class ExpressionExtensions
{
    public static string GetPropertyName<TDelegate>(this Expression<TDelegate> propertyExpr)
    {
        if (propertyExpr is null)
        {
            throw new ArgumentNullException(nameof(propertyExpr));
        }

        if (propertyExpr.Body is MemberExpression memberExpr)
        {
            return memberExpr.Member.Name;
        }

        throw new ArgumentException("Value must contain a body that is a member expression.", nameof(propertyExpr));
    }
}
