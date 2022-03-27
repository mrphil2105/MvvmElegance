using System.Collections;
using System.ComponentModel;
using System.Linq.Expressions;

namespace MvvmElegance;

public abstract class ValidatingModelBase : PropertyChangedBase, INotifyDataErrorInfo
{
    private readonly Dictionary<string, List<string>?> _propertyErrors;
    private readonly SemaphoreSlim _propertyErrorsLock;

    private IModelValidator? _validator;

    protected ValidatingModelBase(IModelValidator? validator = null)
    {
        _propertyErrors = new Dictionary<string, List<string>?>();
        _propertyErrorsLock = new SemaphoreSlim(1, 1);

        // We do not set 'Validator' here as it is virtual.
        _validator = validator;
        _validator?.Initialize(this);

        AutoValidate = true;
    }

    protected virtual IModelValidator? Validator
    {
        get => _validator;
        set
        {
            _validator = value;
            _validator?.Initialize(this);
        }
    }

    protected bool AutoValidate { get; set; }

    public virtual bool HasErrors
    {
        get
        {
            _propertyErrorsLock.Wait();

            try
            {
                return _propertyErrors.Values.Any(e => e is { Count: > 0 });
            }
            finally
            {
                _propertyErrorsLock.Release();
            }
        }
    }

    public event EventHandler<DataErrorsChangedEventArgs>? ErrorsChanged;

    public virtual IEnumerable? GetErrors(string? propertyName)
    {
        _propertyErrorsLock.Wait();

        try
        {
            _propertyErrors.TryGetValue(propertyName ?? string.Empty, out var errors);

            return errors;
        }
        finally
        {
            _propertyErrorsLock.Release();
        }
    }

    protected virtual async Task<bool> ValidateAsync(CancellationToken cancellationToken = default)
    {
        if (Validator == null)
        {
            throw new InvalidOperationException("Unable to validate when there is no validator set for the model.");
        }

        var result = await Validator.ValidateAsync(cancellationToken)
            .ConfigureAwait(false);
        var changedProperties = new List<string>();

        await _propertyErrorsLock.WaitAsync(cancellationToken)
            .ConfigureAwait(false);

        try
        {
            if (result != null)
            {
                foreach (var kvp in result)
                {
                    string propertyName = kvp.Key ?? string.Empty;
                    var newErrors = kvp.Value?.ToList();

                    if (!_propertyErrors.ContainsKey(propertyName))
                    {
                        _propertyErrors.Add(propertyName, newErrors);
                    }
                    else if (AreErrorsEqual(_propertyErrors[propertyName], newErrors))
                    {
                        continue;
                    }
                    else
                    {
                        _propertyErrors[propertyName] = newErrors;
                    }

                    changedProperties.Add(propertyName);
                }
            }

            var removedProperties = _propertyErrors.Keys.Except(result?.Keys ?? Enumerable.Empty<string>())
                .ToList();

            foreach (string propertyName in removedProperties)
            {
                _propertyErrors.Remove(propertyName);
                changedProperties.Add(propertyName);
            }
        }
        finally
        {
            _propertyErrorsLock.Release();
        }

        if (changedProperties.Count > 0)
        {
            OnValidationStateChanged(changedProperties);
        }

        return !HasErrors;
    }

    protected virtual async Task<bool> ValidatePropertyAsync(string? propertyName,
        CancellationToken cancellationToken = default)
    {
        if (Validator == null)
        {
            throw new InvalidOperationException("Unable to validate when there is no validator set for the model.");
        }

        propertyName ??= string.Empty;

        var result = await Validator.ValidatePropertyAsync(propertyName, cancellationToken)
            .ConfigureAwait(false);
        var newErrors = result?.ToList();
        bool haveErrorsChanged = false;

        await _propertyErrorsLock.WaitAsync(cancellationToken)
            .ConfigureAwait(false);

        try
        {
            if (!_propertyErrors.ContainsKey(propertyName))
            {
                _propertyErrors.Add(propertyName, newErrors);
                haveErrorsChanged = true;
            }
            else if (!AreErrorsEqual(_propertyErrors[propertyName], newErrors))
            {
                _propertyErrors[propertyName] = newErrors;
                haveErrorsChanged = true;
            }
        }
        finally
        {
            _propertyErrorsLock.Release();
        }

        if (haveErrorsChanged)
        {
            OnValidationStateChanged(new[] { propertyName });
        }

        return newErrors == null || newErrors.Count == 0;
    }

    protected virtual Task<bool> ValidatePropertyAsync<TProperty>(Expression<Func<TProperty>> propertyExpr,
        CancellationToken cancellationToken = default)
    {
        return ValidatePropertyAsync(propertyExpr.GetPropertyName(), cancellationToken);
    }

    protected virtual void RaiseErrorsChanged(string? propertyName)
    {
        OnErrorsChanged(new DataErrorsChangedEventArgs(propertyName));
    }

    protected virtual void RaiseErrorsChanged<TProperty>(Expression<Func<TProperty>> propertyExpr)
    {
        RaiseErrorsChanged(propertyExpr.GetPropertyName());
    }

    protected virtual void OnValidationStateChanged(IEnumerable<string> changedProperties)
    {
        RaisePropertyChanged(nameof(HasErrors));

        foreach (string propertyName in changedProperties)
        {
            RaiseErrorsChanged(propertyName);
        }
    }

    protected virtual void OnErrorsChanged(DataErrorsChangedEventArgs e)
    {
        ErrorsChanged?.Invoke(this, e);
    }

    protected override async void OnPropertyChanged(PropertyChangedEventArgs e)
    {
        base.OnPropertyChanged(e);

        if (Validator != null && AutoValidate && e.PropertyName != nameof(HasErrors))
        {
            await ValidatePropertyAsync(e.PropertyName)
                .ConfigureAwait(false);
        }
    }

    private static bool AreErrorsEqual(IEnumerable<string>? left, IEnumerable<string>? right)
    {
        if (left == null ^ right == null)
        {
            return false;
        }

        return left == null || left.SequenceEqual(right!);
    }
}
