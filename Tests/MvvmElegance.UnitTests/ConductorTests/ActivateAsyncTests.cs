namespace MvvmElegance.UnitTests.ConductorTests;

public class ActivateAsyncTests
{
    [Theory]
    [AutoData]
    public async Task ActivateAsync_ActivatesActiveItem_WhenInactive(Conductor<Screen> conductor)
    {
        await ScreenExtensions.TryActivateAsync(conductor);

        conductor.ActiveItem!.State.Should().Be(ScreenState.Active);
    }
}
