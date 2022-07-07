namespace MvvmElegance.UnitTests.ConductorOneActiveTests;

public class ActivateAsyncTests
{
    [Theory]
    [AutoData]
    public async Task ActivateAsync_ActivatesActiveItem_WhenInactive(Conductor<Screen>.Collection.OneActive conductor)
    {
        await ScreenExtensions.TryActivateAsync(conductor);

        conductor.ActiveItem!.State.Should()
            .Be(ScreenState.Active);
    }
}
