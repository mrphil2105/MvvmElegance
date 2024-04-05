namespace MvvmElegance.UnitTests.PropertyChangedBaseTests;

public class SetTests
{
    [Theory]
    [AutoData]
    public void Set_SetsValue_WhenPropertyIsWrapped(ViewModel viewModel, string value)
    {
        viewModel.Foo = value;

        viewModel.Foo.Should().Be(value);
    }

    [Theory]
    [AutoData]
    public void Set_SetsValue_WhenFieldIsWrapped(ViewModel viewModel, string value)
    {
        viewModel.Bar = value;

        viewModel.Bar.Should().Be(value);
    }

    [Theory]
    [AutoData]
    public void Set_RaisesPropertyChanged_WhenPropertyIsChanged(ViewModel viewModel, string value)
    {
        using var monitor = viewModel.Monitor();

        viewModel.Foo = value;

        monitor.Should().RaisePropertyChangeFor(vm => vm.Foo);
    }

    [Theory]
    [AutoData]
    public void Set_RaisesPropertyChanged_WhenFieldIsChanged(ViewModel viewModel, string value)
    {
        using var monitor = viewModel.Monitor();

        viewModel.Bar = value;

        monitor.Should().RaisePropertyChangeFor(vm => vm.Bar);
    }

    [Theory]
    [AutoData]
    public void Set_DoesNotRaisePropertyChanged_WhenValueIsEqual(ViewModel viewModel)
    {
        using var monitor = viewModel.Monitor();

        viewModel.Foo = viewModel.Foo;

        monitor.Should().NotRaisePropertyChangeFor(vm => vm.Foo);
    }
}
