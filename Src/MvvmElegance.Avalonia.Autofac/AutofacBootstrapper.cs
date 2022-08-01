using Avalonia.Controls.ApplicationLifetimes;
using MvvmElegance.Internal;
using MvvmElegance.Xaml;

namespace MvvmElegance;

/// <summary>
/// Provides a bootstrapper that uses Autofac to configure basic services and launch the application.
/// </summary>
/// <typeparam name="TRootViewModel">The main view model type for the main view.</typeparam>
public abstract class AutofacBootstrapper<TRootViewModel> : BootstrapperBase
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

    /// <summary>
    /// Launches the application and displays the root view.
    /// </summary>
    /// <exception cref="InvalidOperationException">The application <see cref="BootstrapperBase" /> has not been initialized.</exception>
    protected override void Launch()
    {
        if (Application.Current!.ApplicationLifetime is not IClassicDesktopStyleApplicationLifetime appLifetime)
        {
            throw new InvalidOperationException("The current application lifetime must be of type " +
                $"'{typeof(IClassicDesktopStyleApplicationLifetime).FullName}'.");
        }

        if (!Application.Current.TryFindResource<IExtendedViewManager>(View.ViewManagerResourceKey,
                out var viewManager))
        {
            throw new InvalidOperationException(
                $"Method '{typeof(AutofacBootstrapper<>).FullName}.{nameof(Launch)}' cannot be called before " +
                $"method '{typeof(BootstrapperBase).FullName}.{nameof(Initialize)}' has been called.");
        }

        var rootViewModel = GetService<TRootViewModel>();
        var rootView = viewManager!.CreateView(rootViewModel);

        if (rootView is not Window window)
        {
            throw new InvalidOperationException($"The root view was of type '{rootView.GetType().FullName}', " +
                $"but must be of type '{typeof(Window).FullName}'.");
        }

        appLifetime.MainWindow = window;
    }

    /// <inheritdoc />
    public override void Dispose()
    {
        _container?.Dispose();
    }
}
