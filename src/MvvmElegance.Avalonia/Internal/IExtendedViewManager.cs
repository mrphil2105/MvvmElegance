namespace MvvmElegance.Internal;

internal interface IExtendedViewManager : IViewManager
{
    Control CreateView(object model);

    void SetModel(Control control, object? value);

    WindowConductor GetWindowConductor(object model);
}
