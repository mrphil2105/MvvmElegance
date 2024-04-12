namespace MvvmElegance.UnitTests.ConductorAllActiveTests;

public class DeactivateAsyncTests
{
    [Theory]
    [AutoData]
    public async Task DeactivateAsync_DeactivatesItems_WhenActive(
        Conductor<Screen>.Collection.AllActive conductor,
        List<Screen> screens
    )
    {
        conductor.Items.AddRange(screens);
        await ScreenExtensions.TryActivateAsync(conductor);

        await ScreenExtensions.TryDeactivateAsync(conductor);

        screens.Should().OnlyContain(s => s.State == ScreenState.Inactive);
    }
}
