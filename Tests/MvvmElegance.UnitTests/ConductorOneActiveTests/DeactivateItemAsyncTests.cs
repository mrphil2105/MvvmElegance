namespace MvvmElegance.UnitTests.ConductorOneActiveTests;

public class DeactivateItemAsyncTests
{
    [Theory]
    [AutoData]
    public async Task DeactivateItemAsync_DeactivatesActiveItem(Conductor<Screen>.Collection.OneActive conductor)
    {
        var activeItem = conductor.ActiveItem!;
        await ScreenExtensions.TryActivateAsync(conductor.ActiveItem);

        await conductor.DeactivateItemAsync(conductor.ActiveItem);

        activeItem.State.Should()
            .Be(ScreenState.Inactive);
    }

    [Theory]
    [AutoData]
    public async Task DeactivateItemAsync_DeactivatesScreen(Conductor<Screen>.Collection.OneActive conductor,
        Screen screen)
    {
        await ScreenExtensions.TryActivateAsync(screen);

        await conductor.DeactivateItemAsync(screen);

        screen.State.Should()
            .Be(ScreenState.Inactive);
    }

    [Theory]
    [AutoData]
    public async Task DeactivateItemAsync_DoesNotSetScreenParent(Conductor<Screen>.Collection.OneActive conductor,
        Screen screen)
    {
        await conductor.DeactivateItemAsync(screen);

        screen.Parent.Should()
            .NotBe(conductor);
    }

    [Theory]
    [AutoData]
    public async Task DeactivateItemAsync_ClearsActiveItem_WhenContainsSingle(
        Conductor<object>.Collection.OneActive conductor)
    {
        await conductor.DeactivateItemAsync(conductor.ActiveItem);

        conductor.ActiveItem.Should()
            .BeNull();
    }

    [Theory]
    [AutoData]
    public async Task DeactivateItemAsync_ChangesActiveItem_WhenContainsMultiple(
        Conductor<object>.Collection.OneActive conductor, List<object> items)
    {
        conductor.Items.AddRange(items);
        await conductor.ActivateItemAsync(items.Last());

        await conductor.DeactivateItemAsync(items.Last());

        conductor.ActiveItem.Should()
            .Be(conductor.Items[^2]);
    }

    [Theory]
    [AutoData]
    public async Task DeactivateItemAsync_DoesNotAddItem(Conductor<object>.Collection.OneActive conductor, object item)
    {
        await conductor.DeactivateItemAsync(item);

        conductor.Items.Should()
            .NotContain(item);
    }
}
