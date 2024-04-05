using System.Collections;
using System.ComponentModel;
using System.Linq.Expressions;

namespace MvvmElegance;

/// <summary>
/// Provides the base class for validating models.
/// </summary>
public abstract class ValidatingModelBase : PropertyChangedBase, INotifyDataErrorInfo
{
    private readonly Dictionary<string, List<string>?> _propertyErrors;
    private readonly SemaphoreSlim _propertyErrorsLock;

    private IModelValidator? _validator;

    /// <summary>
    /// Initializes a new instance of the <see cref="ValidatingModelBase" /> class with the specified validator.
    /// </summary>
    /// <param name="validator">The validator to use for validation.</param>
    protected ValidatingModelBase(IModelValidator? validator = null)
    {
        _propertyErrors = new Dictionary<string, List<string>?>();
        _propertyErrorsLock = new SemaphoreSlim(1, 1);

        // We do not set 'Validator' here as it is virtual.
        _validator = validator;
        _validator?.Initialize(this);

        AutoValidate = true;
    }

    /// <summary>
    /// Gets or sets the validator to use for validation.
    /// </summary>
    protected virtual IModelValidator? Validator
    {
        get => _validator;
        set
        {
            _validator = value;
            _validator?.Initialize(this);
        }
    }

    /// <summary>
    /// Gets or sets a boolean indicating whether automatic validation should be performed on property change.
    /// </summary>
    protected bool AutoValidate { get; set; }

    /// <summary>
    /// Gets a boolean indicating whether the model has errors.
    /// </summary>
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

    /// <summary>
    /// An event that is raised when the errors of the model have changed.
    /// </summary>
    public event EventHandler<DataErrorsChangedEventArgs>? ErrorsChanged;

    /// <summary>
    /// Returns the errors for a specified property or the entire model.
    /// </summary>
    /// <param name="propertyName">The name of the property to return errors for or <c>null</c> for the entire model.</param>
    /// <returns>A collection of errors for the specified property or the entire model or <c>null</c> if there are none.</returns>
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

    /// <summary>
    /// Validates the model and its properties.
    /// </summary>
    /// <param name="cancellationToken">The cancellation token used to cancel the operation.</param>
    /// <returns>A task representing the asynchronous operation, with a boolean indicating whether the model has errors.</returns>
    /// <exception cref="InvalidOperationException">There is no validator set for the model.</exception>
    protected virtual async Task<bool> ValidateAsync(CancellationToken cancellationToken = default)
    {
        if (Validator == null)
        {
            throw new InvalidOperationException("Unable to validate when there is no validator set for the model.");
        }

        var result = await Validator.ValidateAsync(cancellationToken).ConfigureAwait(false);
        var changedProperties = new List<string>();

        await _propertyErrorsLock.WaitAsync(cancellationToken).ConfigureAwait(false);

        try
        {
            if (result != null)
            {
                foreach (var kvp in result)
                {
                    var propertyName = kvp.Key ?? string.Empty;
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

            var removedProperties = _propertyErrors.Keys.Except(result?.Keys ?? Enumerable.Empty<string>()).ToList();

            foreach (var propertyName in removedProperties)
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

    /// <summary>
    /// Validates the property with the specified name.
    /// </summary>
    /// <param name="propertyName">The name of the property to validate.</param>
    /// <param name="cancellationToken">The cancellation token used to cancel the operation.</param>
    /// <returns>A task representing the asynchronous operation, with a boolean indicating whether the property has errors.</returns>
    /// <exception cref="InvalidOperationException">There is no validator set for the model.</exception>
    protected virtual async Task<bool> ValidatePropertyAsync(
        string? propertyName,
        CancellationToken cancellationToken = default
    )
    {
        if (Validator == null)
        {
            throw new InvalidOperationException("Unable to validate when there is no validator set for the model.");
        }

        propertyName ??= string.Empty;

        var result = await Validator.ValidatePropertyAsync(propertyName, cancellationToken).ConfigureAwait(false);
        var newErrors = result?.ToList();
        var haveErrorsChanged = false;

        await _propertyErrorsLock.WaitAsync(cancellationToken).ConfigureAwait(false);

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

    /// <summary>
    /// Validates the property of the specified expression.
    /// </summary>
    /// <param name="propertyExpr">The expression containing the property to validate.</param>
    /// <param name="cancellationToken">The cancellation token used to cancel the operation.</param>
    /// <typeparam name="TProperty">The type of the specified property.</typeparam>
    /// <returns>A task representing the asynchronous operation, with a boolean indicating whether the property has errors.</returns>
    /// <exception cref="InvalidOperationException">There is no validator set for the model.</exception>
    protected virtual Task<bool> ValidatePropertyAsync<TProperty>(
        Expression<Func<TProperty>> propertyExpr,
        CancellationToken cancellationToken = default
    )
    {
        return ValidatePropertyAsync(propertyExpr.GetPropertyName(), cancellationToken);
    }

    /// <summary>
    /// Raises the <see cref="ErrorsChanged" /> event with the specified property name.
    /// </summary>
    /// <param name="propertyName">The name of the property with changed errors.</param>
    protected virtual void RaiseErrorsChanged(string? propertyName)
    {
        OnErrorsChanged(new DataErrorsChangedEventArgs(propertyName));
    }

    /// <summary>
    /// Raises the <see cref="ErrorsChanged" /> event with the name of the property of the specified expression.
    /// </summary>
    /// <param name="propertyExpr">The expression containing the property with changed errors.</param>
    /// <typeparam name="TProperty">The type of the specified property.</typeparam>
    protected virtual void RaiseErrorsChanged<TProperty>(Expression<Func<TProperty>> propertyExpr)
    {
        RaiseErrorsChanged(propertyExpr.GetPropertyName());
    }

    /// <summary>
    /// Raises the <see cref="PropertyChangedBase.PropertyChanged" /> event for <see cref="HasErrors" /> and raises the
    /// <see cref="ErrorsChanged" /> event for the specified changed properties.
    /// </summary>
    /// <param name="changedProperties">The collection of names of the properties with changed errors.</param>
    protected virtual void OnValidationStateChanged(IEnumerable<string> changedProperties)
    {
        RaisePropertyChanged(nameof(HasErrors));

        foreach (var propertyName in changedProperties)
        {
            RaiseErrorsChanged(propertyName);
        }
    }

    /// <summary>
    /// Raises the <see cref="ErrorsChanged" /> event with the specified arguments.
    /// </summary>
    /// <param name="e">The arguments of the event.</param>
    protected virtual void OnErrorsChanged(DataErrorsChangedEventArgs e)
    {
        ErrorsChanged?.Invoke(this, e);
    }

    /// <inheritdoc />
    protected override async void OnPropertyChanged(PropertyChangedEventArgs e)
    {
        base.OnPropertyChanged(e);

        if (Validator != null && AutoValidate && e.PropertyName != nameof(HasErrors))
        {
            await ValidatePropertyAsync(e.PropertyName).ConfigureAwait(false);
        }
    }

    private static bool AreErrorsEqual(IEnumerable<string>? left, IEnumerable<string>? right)
    {
        if ((left == null) ^ (right == null))
        {
            return false;
        }

        return left?.SequenceEqual(right!) != false;
    }
}
