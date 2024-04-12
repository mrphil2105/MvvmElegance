namespace MvvmElegance;

/// <summary>
/// Represents a combination of all expected behaviors of a screen.
/// </summary>
public interface IScreen : IHaveDisplayName, IScreenState, IChild, IGuardClose, IRequestClose { }
