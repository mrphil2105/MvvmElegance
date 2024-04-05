using System.Reflection;

namespace MvvmElegance;

/// <summary>
/// Provides a type locator for views that uses naming convention.
/// </summary>
public class DefaultViewTypeLocator : IViewTypeLocator
{
    private const string ViewModelSuffix = "ViewModel";
    private const string ViewSuffix = "View";

    private readonly Assembly _viewAssembly;

    /// <summary>
    /// Initializes a new instance of the <see cref="DefaultViewTypeLocator" /> class with the specified view assembly.
    /// </summary>
    /// <param name="viewAssembly">The view assembly that contains the view types.</param>
    public DefaultViewTypeLocator(Assembly viewAssembly)
    {
        _viewAssembly = viewAssembly;
    }

    /// <inheritdoc />
    /// <exception cref="ViewLocationException">The view model type name does not end in 'ViewModel' or
    /// a matching view type could not be found or more than one view type was found.</exception>
    public Type Locate(Type modelType)
    {
        var viewModelName = modelType.Name;

        if (!viewModelName.EndsWith(ViewModelSuffix, StringComparison.Ordinal))
        {
            throw new ViewLocationException($"The view model type name must end with '{ViewModelSuffix}'.", modelType);
        }

        var viewName = viewModelName.Remove(viewModelName.Length - ViewModelSuffix.Length) + ViewSuffix;
        var viewTypes = _viewAssembly.ExportedTypes.Where(t => t.Name == viewName).ToList();

        return viewTypes.Count switch
        {
            0
                => throw new ViewLocationException(
                    $"Unable to find a view type with name '{viewName}' for view model type '{modelType.FullName}'.",
                    modelType
                ),
            > 1
                => throw new ViewLocationException(
                    $"More than one view type with name '{viewName}' was found for view model type "
                        + $"'{modelType.FullName}'.",
                    modelType
                ),
            _ => viewTypes[0]
        };
    }
}
