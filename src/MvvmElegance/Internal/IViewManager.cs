namespace MvvmElegance.Internal;

internal interface IViewManager
{
    void CloseView(object model, bool? dialogResult = null);
}
