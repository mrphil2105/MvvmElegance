using System.Reflection;
using AutoFixture;
using AutoFixture.Kernel;

namespace MvvmElegance.UnitTests;

public class CollectionSizeAttribute : CustomizeAttribute
{
    private readonly int _size;

    public CollectionSizeAttribute(int size)
    {
        if (size < 0)
        {
            throw new ArgumentOutOfRangeException(nameof(size), "Value cannot be smaller than 0.");
        }

        _size = size;
    }

    public override ICustomization GetCustomization(ParameterInfo parameter)
    {
        if (parameter is null)
        {
            throw new ArgumentNullException(nameof(parameter));
        }

        if (!parameter.ParameterType.IsGenericType)
        {
            throw new ArgumentException("The parameter must be a generic collection type.", nameof(parameter));
        }

        var genericArgumentType = parameter.ParameterType.GetGenericArguments()[0];
        var listType = typeof(List<>).MakeGenericType(genericArgumentType);

        if (!parameter.ParameterType.IsAssignableFrom(listType))
        {
            throw new ArgumentException($"The parameter must be assignable from: {listType}.");
        }

        var customizationType = typeof(CollectionSizeCustomization<>).MakeGenericType(genericArgumentType);

        return (ICustomization)Activator.CreateInstance(customizationType, parameter, _size)!;
    }

    private class CollectionSizeCustomization<T> : ICustomization
    {
        private readonly ParameterInfo _parameter;
        private readonly int _size;

        public CollectionSizeCustomization(ParameterInfo parameter, int size)
        {
            _parameter = parameter;
            _size = size;
        }

        public void Customize(IFixture fixture)
        {
            var listBuilder = new FixedBuilder(fixture.CreateMany<T>(_size)
                .ToList());
            var filteringBuilder = new FilteringSpecimenBuilder(listBuilder, new EqualRequestSpecification(_parameter));
            fixture.Customizations.Add(filteringBuilder);
        }
    }
}
