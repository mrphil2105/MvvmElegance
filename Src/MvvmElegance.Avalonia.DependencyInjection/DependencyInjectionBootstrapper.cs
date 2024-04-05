using MvvmElegance.Internal;

namespace MvvmElegance;

/// <summary>
/// Provides a bootstrapper that uses Microsoft.Extensions.DependencyInjection to configure basic services and launch the application.
/// </summary>
/// <typeparam name="TRootViewModel">The main view model type for the main view.</typeparam>
public abstract class DependencyInjectionBootstrapper<TRootViewModel> : BootstrapperBase<TRootViewModel>
    where TRootViewModel : notnull
{
    private ServiceProvider? _serviceProvider;

    /// <inheritdoc />
    /// <exception cref="InvalidOperationException">Method <see cref="ConfigureServices()" /> has not been called.</exception>
    public override TService GetService<TService>()
    {
        if (_serviceProvider == null)
        {
            throw new InvalidOperationException($"Method '{typeof(DependencyInjectionBootstrapper<>).FullName}." +
                $"{nameof(GetService)}' cannot be called before method " +
                $"'{typeof(DependencyInjectionBootstrapper<>).FullName}.{nameof(ConfigureServices)}' has been called.");
        }

        return _serviceProvider.GetRequiredService<TService>();
    }

    /// <summary>
    /// Configures the application services.
    /// </summary>
    protected override void ConfigureServices()
    {
        var services = new ServiceCollection();
        AddServices(services);
        ConfigureServices(services);
        _serviceProvider = services.BuildServiceProvider();
    }

    /// <summary>
    /// Called when additional application services should be configured.
    /// </summary>
    /// <param name="services">The service collection for the service provider.</param>
    protected virtual void ConfigureServices(IServiceCollection services)
    {
    }

    private void AddServices(IServiceCollection services)
    {
        var appAssembly = GetType()
            .Assembly;

        services.AddTransient<ServiceFactory>(p => p.GetRequiredService);

        services.AddSingleton<IEventAggregator, EventAggregator>();

        services.AddTransient<ITimer, Timer>();

        services.AddTransient<IViewTypeLocator>(_ => new DefaultViewTypeLocator(appAssembly));

        services.AddTransient<IViewService, ViewService>();
    }

    /// <inheritdoc />
    public override void Dispose()
    {
        _serviceProvider?.Dispose();
    }
}
