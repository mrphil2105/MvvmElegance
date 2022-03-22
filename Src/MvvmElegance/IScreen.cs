namespace MvvmElegance;

public interface IScreen : IHaveDisplayName, IScreenState, IChild, IGuardClose, IRequestClose
{
}
