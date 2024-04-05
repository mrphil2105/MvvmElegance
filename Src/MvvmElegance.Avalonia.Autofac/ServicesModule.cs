using MvvmElegance.Internal;

namespace MvvmElegance;

internal class ServicesModule : Module
{
    private readonly IBootstrapper _bootstrapper;

    public ServicesModule(IBootstrapper bootstrapper)
    {
        _bootstrapper = bootstrapper;
    }

    protected override void Load(ContainerBuilder builder)
    {
        var appAssembly = _bootstrapper.GetType().Assembly;

        builder.Register<ServiceFactory>(c =>
        {
            var innerContext = c.Resolve<IComponentContext>();

            return t => innerContext.Resolve(t);
        });

        builder.RegisterType<EventAggregator>().As<IEventAggregator>().SingleInstance();

        builder.RegisterType<Timer>().As<ITimer>();

        builder.Register(_ => new DefaultViewTypeLocator(appAssembly)).As<IViewTypeLocator>();

        builder.RegisterType<ViewService>().As<IViewService>();
    }
}
