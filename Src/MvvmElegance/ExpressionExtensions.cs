using System.Linq.Expressions;

namespace MvvmElegance;

/// <summary>
/// Provides extensions for expressions.
/// </summary>
public static class ExpressionExtensions
{
    /// <summary>
    /// Returns the name of the property of the specified expression.
    /// </summary>
    /// <param name="propertyExpr">The expression containing the property to return the name of.</param>
    /// <typeparam name="TDelegate">The type of the delegate that contains the specified property.</typeparam>
    /// <returns>A string with the name of the specified property.</returns>
    /// <exception cref="ArgumentNullException">Value of parameter <paramref name="propertyExpr" /> is <c>null</c>.</exception>
    /// <exception cref="ArgumentException">Value of parameter <paramref name="propertyExpr" /> does not contain a body that is a member expression.</exception>
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
