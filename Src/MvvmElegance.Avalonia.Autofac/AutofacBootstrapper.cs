namespace MvvmElegance;

/// <summary>
/// Provides a bootstrapper that uses Autofac to configure basic services and launch the application.
/// </summary>
/// <typeparam name="TRootViewModel">The main view model type for the main view.</typeparam>
public abstract class AutofacBootstrapper<TRootViewModel> : BootstrapperBase<TRootViewModel>
    where TRootViewModel : notnull
{
    private IContainer? _container;

    /// <inheritdoc />
    /// <exception cref="InvalidOperationException">Method <see cref="ConfigureServices()" /> has not been called.</exception>
    public override TService GetService<TService>()
    {
        if (_container == null)
        {
            throw new InvalidOperationException(
                $"Method '{typeof(AutofacBootstrapper<>).FullName}.{nameof(GetService)}' cannot be called before " +
                $"method '{typeof(AutofacBootstrapper<>).FullName}.{nameof(ConfigureServices)}' has been called.");
        }

        return _container.Resolve<TService>();
    }

    /// <summary>
    /// Configures the application services.
    /// </summary>
    protected override void ConfigureServices()
    {
        var builder = new ContainerBuilder();
        builder.RegisterModule(new ServicesModule(this));
        ConfigureServices(builder);
        _container = builder.Build();
    }

    /// <summary>
    /// Called when additional application services should be configured.
    /// </summary>
    /// <param name="builder">The builder for the Autofac container.</param>
    protected virtual void ConfigureServices(ContainerBuilder builder)
    {
    }

    /// <inheritdoc />
    public override void Dispose()
    {
        _container?.Dispose();
    }
}
