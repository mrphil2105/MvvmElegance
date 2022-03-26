namespace MvvmElegance;

// TODO: Make 'Screen' inherit from 'ValidatingModelBase' when it has been created.
public class Screen : PropertyChangedBase, IScreen
{
    private bool _isInitialized;
    private ScreenState _state;
    private string? _displayName;
    private object? _parent;

    public Screen()
    {
        _displayName = GetType()
            .FullName;
        _state = ScreenState.Inactive;
    }

    public virtual bool IsInitialized
    {
        get => _isInitialized;
        private set => Set(ref _isInitialized, value);
    }

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

    public virtual string? DisplayName
    {
        get => _displayName;
        set => Set(ref _displayName, value);
    }

    public virtual object? Parent
    {
        get => _parent;
        set => Set(ref _parent, value);
    }

    public virtual bool IsActive => State == ScreenState.Active;

    public virtual bool IsClosed => State == ScreenState.Closed;

    public event EventHandler<ActivatedEventArgs>? Activated;

    public event EventHandler<DeactivatedEventArgs>? Deactivated;

    public event EventHandler<ClosedEventArgs>? Closed;

    public virtual Task<bool> CanCloseAsync(CancellationToken cancellationToken = default)
    {
        return Task.FromResult(true);
    }

    public virtual Task<bool> TryCloseAsync(bool? dialogResult = null, CancellationToken cancellationToken = default)
    {
        throw new NotImplementedException();
    }

    protected virtual Task OnInitializeAsync(CancellationToken cancellationToken = default)
    {
        return Task.CompletedTask;
    }

    protected virtual Task OnActivateAsync(CancellationToken cancellationToken = default)
    {
        return Task.CompletedTask;
    }

    protected virtual Task OnDeactivateAsync(CancellationToken cancellationToken = default)
    {
        return Task.CompletedTask;
    }

    protected virtual Task OnCloseAsync(CancellationToken cancellationToken = default)
    {
        return Task.CompletedTask;
    }

    protected virtual void OnActivated(ActivatedEventArgs e)
    {
        Activated?.Invoke(this, e);
    }

    protected virtual void OnDeactivated(DeactivatedEventArgs e)
    {
        Deactivated?.Invoke(this, e);
    }

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
