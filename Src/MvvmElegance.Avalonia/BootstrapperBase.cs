using System.Collections.ObjectModel;
using Avalonia.Controls.ApplicationLifetimes;
using Avalonia.Threading;
using MvvmElegance.Internal;
using MvvmElegance.Xaml;

namespace MvvmElegance;

/// <summary>
/// Provides a bootstrapper that can set up services and launch the application.
/// </summary>
public abstract class BootstrapperBase : IBootstrapper
{
    /// <summary>
    /// Gets the command line arguments that were passed to the application on execute.
    /// </summary>
    public IReadOnlyList<string>? Args { get; private set; }

    /// <summary>
    /// Gets a service instance of the specified type.
    /// </summary>
    /// <typeparam name="TService">The type of the service to create.</typeparam>
    /// <returns>An instance of the specified service type.</returns>
    public abstract TService GetService<TService>()
        where TService : notnull;

    /// <inheritdoc />
    public void Initialize()
    {
        if (Application.Current!.ApplicationLifetime is not IClassicDesktopStyleApplicationLifetime appLifetime)
        {
            throw new InvalidOperationException("The current application lifetime must be of type " +
                $"'{typeof(IClassicDesktopStyleApplicationLifetime).FullName}'.");
        }

        var dispatcherWrapper = new DispatcherWrapper(Dispatcher.UIThread);
        Dispatch.Initialize(dispatcherWrapper);

        appLifetime.Startup += (_, e) => Start(e.Args);
        appLifetime.Exit += (_, _) => Dispose();
    }

    /// <summary>
    /// Called when the application services should be configured.
    /// </summary>
    protected abstract void ConfigureServices();

    /// <summary>
    /// Called when the application should be launched and is responsible for displaying the root view.
    /// </summary>
    protected abstract void Launch();

    /// <summary>
    /// Called after the application services have been configured.
    /// </summary>
    protected virtual void Configure()
    {
    }

    private void Start(string[] args)
    {
        if (args is null)
        {
            throw new ArgumentNullException(nameof(args));
        }

        Args = new ReadOnlyCollection<string>(args);

        ConfigureServices();

        var viewManager = new ExtendedViewManager(GetService<ServiceFactory>(), GetService<IViewTypeLocator>());
        ViewManager.Current = viewManager;
        Application.Current!.Resources.Add(View.ViewManagerResourceKey, viewManager);

        Configure();
        Launch();
    }

    /// <summary>
    /// Performs application-defined tasks associated with freeing, releasing, or resetting unmanaged resources.
    /// </summary>
    public virtual void Dispose()
    {
    }
}
