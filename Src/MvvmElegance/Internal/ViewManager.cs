namespace MvvmElegance.Internal;

internal static class ViewManager
{
    private static IViewManager? _current;

    public static IViewManager Current
    {
        get
        {
            if (_current == null)
            {
                throw new InvalidOperationException("Value cannot be retrieved before method " +
                    $"'{typeof(IBootstrapper).FullName}.{nameof(IBootstrapper.Initialize)}' has been called.");
            }

            return _current;
        }
        set
        {
            if (_current != null)
            {
                throw new InvalidOperationException("Value can only be set once.");
            }

            _current = value;
        }
    }
}
