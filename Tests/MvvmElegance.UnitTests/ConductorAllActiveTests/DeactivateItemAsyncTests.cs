namespace MvvmElegance.UnitTests.ConductorAllActiveTests;

public class DeactivateItemAsyncTests
{
    [Theory]
    [AutoData]
    public async Task DeactivateItemAsync_DeactivatesScreen(
        Conductor<Screen>.Collection.AllActive conductor,
        Screen screen
    )
    {
        await ScreenExtensions.TryActivateAsync(screen);

        await conductor.DeactivateItemAsync(screen);

        screen.State.Should().Be(ScreenState.Inactive);
    }

    [Theory]
    [AutoData]
    public async Task DeactivateItemAsync_DoesNotSetScreenParent(
        Conductor<Screen>.Collection.AllActive conductor,
        Screen screen
    )
    {
        await conductor.DeactivateItemAsync(screen);

        screen.Parent.Should().NotBe(conductor);
    }

    [Theory]
    [AutoData]
    public async Task DeactivateItemAsync_DoesNotAddItem(Conductor<object>.Collection.AllActive conductor, object item)
    {
        await conductor.DeactivateItemAsync(item);

        conductor.Items.Should().NotContain(item);
    }
}
