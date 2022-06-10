namespace MvvmElegance;

public interface IParent<out T> : IParent
{
    new IEnumerable<T>? GetChildren();
}
