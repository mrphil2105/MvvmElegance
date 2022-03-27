namespace MvvmElegance;

/// <summary>
/// Provides an implementation of all expected behaviors of a screen.
/// </summary>
// TODO: Make 'Screen' inherit from 'ValidatingModelBase' when it has been created.
public class Screen : PropertyChangedBase, IScreen
{
    private bool _isInitialized;
    private ScreenState _state;
    private string? _displayName;
    private object? _parent;

    /// <summary>
    /// Initializes a new instance of the <see cref="Screen" /> class.
    /// </summary>
    public Screen()
    {
        _displayName = GetType()
            .FullName;
        _state = ScreenState.Inactive;
    }

    /// <summary>
    /// Gets a boolean indicating whether the screen has been initialized.
    /// </summary>
    public virtual bool IsInitialized
    {
        get => _isInitialized;
        private set => Set(ref _isInitialized, value);
    }

    /// <inheritdoc />
    public virtual ScreenState State
    {
        get => _state;
        private set
        {
            if (!Set(ref _state, value))
            {
                return;
            }

            RaisePropertyChanged(nameof(IsActive));
            RaisePropertyChanged(nameof(IsClosed));
        }
    }

    /// <inheritdoc />
    public virtual string? DisplayName
    {
        get => _displayName;
        set => Set(ref _displayName, value);
    }

    /// <inheritdoc />
    public virtual object? Parent
    {
        get => _parent;
        set => Set(ref _parent, value);
    }

    /// <summary>
    /// Gets a boolean indicating whether the screen is active.
    /// </summary>
    public virtual bool IsActive => State == ScreenState.Active;

    /// <summary>
    /// Gets a boolean indicating whether the screen is closed.
    /// </summary>
    public virtual bool IsClosed => State == ScreenState.Closed;

    /// <inheritdoc />
    public event EventHandler<ActivatedEventArgs>? Activated;

    /// <inheritdoc />
    public event EventHandler<DeactivatedEventArgs>? Deactivated;

    /// <inheritdoc />
    public event EventHandler<ClosedEventArgs>? Closed;

    /// <inheritdoc />
    public virtual Task<bool> CanCloseAsync(CancellationToken cancellationToken = default)
    {
        return Task.FromResult(true);
    }

    /// <inheritdoc />
    public virtual Task<bool> TryCloseAsync(bool? dialogResult = null, CancellationToken cancellationToken = default)
    {
        throw new NotImplementedException();
    }

    /// <summary>
    /// Called when the screen is initializing.
    /// </summary>
    /// <param name="cancellationToken">The cancellation token used to cancel the operation.</param>
    /// <returns>A task representing the asynchronous operation.</returns>
    protected virtual Task OnInitializeAsync(CancellationToken cancellationToken = default)
    {
        return Task.CompletedTask;
    }

    /// <summary>
    /// Called when the screen is activating.
    /// </summary>
    /// <param name="cancellationToken">The cancellation token used to cancel the operation.</param>
    /// <returns>A task representing the asynchronous operation.</returns>
    protected virtual Task OnActivateAsync(CancellationToken cancellationToken = default)
    {
        return Task.CompletedTask;
    }

    /// <summary>
    /// Called when the screen is deactivating.
    /// </summary>
    /// <param name="cancellationToken">The cancellation token used to cancel the operation.</param>
    /// <returns>A task representing the asynchronous operation.</returns>
    protected virtual Task OnDeactivateAsync(CancellationToken cancellationToken = default)
    {
        return Task.CompletedTask;
    }

    /// <summary>
    /// Called when the screen is closing.
    /// </summary>
    /// <param name="cancellationToken">The cancellation token used to cancel the operation.</param>
    /// <returns>A task representing the asynchronous operation.</returns>
    protected virtual Task OnCloseAsync(CancellationToken cancellationToken = default)
    {
        return Task.CompletedTask;
    }

    /// <summary>
    /// Raises the <see cref="Activated" /> event with the specified arguments.
    /// </summary>
    /// <param name="e">The arguments of the event.</param>
    protected virtual void OnActivated(ActivatedEventArgs e)
    {
        Activated?.Invoke(this, e);
    }

    /// <summary>
    /// Raises the <see cref="Deactivated" /> event with the specified arguments.
    /// </summary>
    /// <param name="e">The arguments of the event.</param>
    protected virtual void OnDeactivated(DeactivatedEventArgs e)
    {
        Deactivated?.Invoke(this, e);
    }

    /// <summary>
    /// Raises the <see cref="Closed" /> event with the specified arguments.
    /// </summary>
    /// <param name="e">The arguments of the event.</param>
    protected virtual void OnClosed(ClosedEventArgs e)
    {
        Closed?.Invoke(this, e);
    }

    async Task IScreenState.ActivateAsync(CancellationToken cancellationToken)
    {
        if (IsActive)
        {
            return;
        }

        bool hasInitialized = false;

        if (!IsInitialized)
        {
            await OnInitializeAsync(cancellationToken);
            IsInitialized = true;

            hasInitialized = true;
        }

        var previousState = State;
        await OnActivateAsync(cancellationToken);
        State = ScreenState.Active;

        OnActivated(new ActivatedEventArgs(hasInitialized, previousState));
    }

    async Task IScreenState.DeactivateAsync(CancellationToken cancellationToken)
    {
        if (IsClosed)
        {
            // We do not want to go from closed to inactive without an activation in between.
            await ((IScreenState)this).ActivateAsync(cancellationToken);
        }

        if (!IsActive)
        {
            return;
        }

        var previousState = State;
        await OnDeactivateAsync(cancellationToken);
        State = ScreenState.Inactive;

        OnDeactivated(new DeactivatedEventArgs(previousState));
    }

    async Task IScreenState.CloseAsync(CancellationToken cancellationToken)
    {
        if (IsActive)
        {
            // We do not want to go from active to closed without a deactivation in between.
            await ((IScreenState)this).DeactivateAsync(cancellationToken);
        }

        if (IsClosed)
        {
            return;
        }

        IsInitialized = false;

        var previousState = State;
        await OnCloseAsync(cancellationToken);
        State = ScreenState.Closed;

        OnClosed(new ClosedEventArgs(previousState));
    }
}
