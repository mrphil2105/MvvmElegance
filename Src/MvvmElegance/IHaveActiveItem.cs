namespace MvvmElegance;

public interface IHaveActiveItem<T>
{
    T? ActiveItem { get; set; }
}
