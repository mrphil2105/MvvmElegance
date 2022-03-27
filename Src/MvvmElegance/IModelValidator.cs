namespace MvvmElegance;

/// <summary>
/// Represents a validator that validates a model.
/// </summary>
public interface IModelValidator
{
    /// <summary>
    /// Called when a validating model initializes its validator.
    /// </summary>
    /// <param name="model">The validating model.</param>
    void Initialize(object model);

    /// <summary>
    /// Validates the model and its properties.
    /// </summary>
    /// <param name="cancellationToken">The cancellation token used to cancel the operation.</param>
    /// <returns>A task representing the asynchronous operation, with a dictionary containing the validation errors of the model and its properties.</returns>
    Task<IReadOnlyDictionary<string, IEnumerable<string>>?>
        ValidateAsync(CancellationToken cancellationToken = default);

    /// <summary>
    /// Validates the property with the specified name.
    /// </summary>
    /// <param name="propertyName">The name of the property to validate.</param>
    /// <param name="cancellationToken">The cancellation token used to cancel the operation.</param>
    /// <returns>A task representing the asynchronous operation, with a collection containing the validation errors of the specified property.</returns>
    Task<IEnumerable<string>?> ValidatePropertyAsync(string? propertyName,
        CancellationToken cancellationToken = default);
}
