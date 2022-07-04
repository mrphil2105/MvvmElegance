namespace MvvmElegance;

public interface IDispatcher
{
    bool IsCurrent { get; }

    void Post(Action action);

    void Send(Action action);
}
