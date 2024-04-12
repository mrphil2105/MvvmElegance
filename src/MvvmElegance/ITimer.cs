namespace MvvmElegance;

/// <summary>
/// Represents a timer that periodically raises an event on the UI thread.
/// </summary>
public interface ITimer
{
    /// <summary>
    /// Gets or sets the interval between timer events.
    /// </summary>
    TimeSpan Interval { get; set; }

    /// <summary>
    /// Gets or sets a boolean indicating whether the timer is enabled.
    /// </summary>
    bool IsEnabled { get; set; }

    /// <summary>
    /// An event that is raised when the timer has elapsed.
    /// </summary>
    event EventHandler Elapsed;

    /// <summary>
    /// Enables the timer to raise events.
    /// </summary>
    void Start();

    /// <summary>
    /// Disables the timer so it cannot raise events.
    /// </summary>
    void Stop();
}
