namespace MvvmElegance;

public interface IModelValidator
{
    void Initialize(object model);

    Task<IReadOnlyDictionary<string, IEnumerable<string>>?>
        ValidateAsync(CancellationToken cancellationToken = default);

    Task<IEnumerable<string>?> ValidatePropertyAsync(string? propertyName,
        CancellationToken cancellationToken = default);
}
