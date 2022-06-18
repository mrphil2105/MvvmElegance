namespace MvvmElegance.UnitTests.ConductorOneActiveTests;

public class DeactivateAsyncTests
{
    [Theory]
    [AutoData]
    public async Task DeactivateAsync_DeactivatesActiveItem_WhenActive(Conductor<Screen>.Collection.OneActive conductor)
    {
        await ScreenExtensions.TryActivateAsync(conductor);

        await ScreenExtensions.TryDeactivateAsync(conductor);

        conductor.ActiveItem!.State.Should()
            .Be(ScreenState.Inactive);
    }
}
