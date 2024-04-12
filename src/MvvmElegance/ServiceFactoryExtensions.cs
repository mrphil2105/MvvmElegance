namespace MvvmElegance;

/// <summary>
/// Provides extensions for service factories.
/// </summary>
public static class ServiceFactoryExtensions
{
    /// <summary>
    /// Creates a service instance of the specified type.
    /// </summary>
    /// <param name="serviceFactory">The service factory to create service instances from.</param>
    /// <typeparam name="TService">The type of the service to create.</typeparam>
    /// <returns>An instance of the specified service type.</returns>
    /// <exception cref="ArgumentNullException">Value of parameter <paramref name="serviceFactory" /> is <c>null</c>.</exception>
    public static TService Create<TService>(this ServiceFactory serviceFactory)
    {
        if (serviceFactory is null)
        {
            throw new ArgumentNullException(nameof(serviceFactory));
        }

        return (TService)serviceFactory(typeof(TService));
    }
}
