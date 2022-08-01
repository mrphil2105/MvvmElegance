namespace MvvmElegance;

/// <summary>
/// Represents a bootstrapper that prepares the application for use.
/// </summary>
public interface IBootstrapper : IDisposable
{
    /// <summary>
    /// Should be called when the application is initializing before startup.
    /// </summary>
    void Initialize();
}
