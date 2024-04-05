namespace MvvmElegance.UnitTests.ValidatingModelBaseTests;

public class ValidatingModel : ValidatingModelBase
{
    public ValidatingModel(IModelValidator? modelValidator)
        : base(modelValidator) { }

    public Task<bool> ValidateAsync()
    {
        return ValidateAsync(default);
    }

    public Task<bool> ValidatePropertyAsync(string? propertyName)
    {
        return ValidatePropertyAsync(propertyName, default);
    }
}
