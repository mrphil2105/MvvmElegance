namespace MvvmElegance.UnitTests.ConductorAllActiveTests;

public class ActivateAsyncTests
{
    [Theory]
    [AutoData]
    public async Task ActivateAsync_ActivatesItems_WhenInactive(
        Conductor<Screen>.Collection.AllActive conductor,
        List<Screen> screens
    )
    {
        conductor.Items.AddRange(screens);

        await ScreenExtensions.TryActivateAsync(conductor);

        screens.Should().OnlyContain(s => s.State == ScreenState.Active);
    }
}
