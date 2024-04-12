namespace MvvmElegance.Xaml;

/// <summary>
/// Provides a resource dictionary that initializes the application bootstrapper.
/// </summary>
public class ApplicationLoader : ResourceDictionary
{
    private IBootstrapper? _bootstrapper;

    /// <summary>
    /// Gets or sets the application bootstrapper and initializes it.
    /// </summary>
    /// <exception cref="InvalidOperationException">Value cannot be retrieved before it has been set or it can only be set once.</exception>
    /// <exception cref="ArgumentNullException">Value of parameter is <c>null</c>.</exception>
    public IBootstrapper Bootstrapper
    {
        get
        {
            if (_bootstrapper == null)
            {
                throw new InvalidOperationException("Value cannot be retrieved before it has been set.");
            }

            return _bootstrapper;
        }
        set
        {
            if (_bootstrapper != null)
            {
                throw new InvalidOperationException("Value can only be set once.");
            }

            _bootstrapper = value ?? throw new ArgumentNullException(nameof(value));
            _bootstrapper.Initialize();
        }
    }
}
