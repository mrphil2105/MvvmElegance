namespace MvvmElegance;

/// <summary>
/// Represents a validator that validates a model.
/// </summary>
/// <typeparam name="T">The type of the model to validate.</typeparam>
// ReSharper disable once UnusedTypeParameter
public interface IModelValidator<in T> : IModelValidator { }
