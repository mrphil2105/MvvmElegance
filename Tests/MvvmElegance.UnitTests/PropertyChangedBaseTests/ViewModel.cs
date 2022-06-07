namespace MvvmElegance.UnitTests.PropertyChangedBaseTests;

public class ViewModel : PropertyChangedBase
{
    private readonly Model _model;

    public ViewModel()
    {
        _model = new Model();
    }

    public string? Foo
    {
        get => _model.Foo;
        set => Set(() => _model.Foo, value);
    }

    public string? Bar
    {
        get => _model.Bar;
        set => Set(() => _model.Bar, value);
    }
}
