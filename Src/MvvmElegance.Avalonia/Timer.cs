using Avalonia.Threading;

namespace MvvmElegance;

/// <summary>
/// Provides a timer that periodically raises an event on the UI thread.
/// </summary>
public class Timer : ITimer
{
    private readonly DispatcherTimer _dispatcherTimer;

    /// <summary>
    /// Initializes a new instance of the <see cref="Timer" /> class.
    /// </summary>
    public Timer()
    {
        _dispatcherTimer = new DispatcherTimer { Interval = TimeSpan.FromMilliseconds(100) };
    }

    /// <inheritdoc />
    public event EventHandler Elapsed
    {
        add => _dispatcherTimer.Tick += value;
        remove => _dispatcherTimer.Tick -= value;
    }

    /// <inheritdoc />
    public TimeSpan Interval
    {
        get => _dispatcherTimer.Interval;
        set => _dispatcherTimer.Interval = value;
    }

    /// <inheritdoc />
    public bool IsEnabled
    {
        get => _dispatcherTimer.IsEnabled;
        set => _dispatcherTimer.IsEnabled = value;
    }

    /// <inheritdoc />
    public void Start()
    {
        _dispatcherTimer.Start();
    }

    /// <inheritdoc />
    public void Stop()
    {
        _dispatcherTimer.Stop();
    }
}
