using System.Reflection;
using Xunit.Sdk;

namespace MvvmElegance.UnitTests;

public class AutoCompositeDataAttribute : DataAttribute
{
    private readonly DataAttribute _autoDataAttribute;
    private readonly DataAttribute _baseAttribute;

    public AutoCompositeDataAttribute(DataAttribute baseAttribute, DataAttribute autoDataAttribute)
    {
        _baseAttribute = baseAttribute ?? throw new ArgumentNullException(nameof(baseAttribute));
        _autoDataAttribute = autoDataAttribute ?? throw new ArgumentNullException(nameof(autoDataAttribute));
    }

    public override IEnumerable<object[]> GetData(MethodInfo testMethod)
    {
        if (testMethod is null)
        {
            throw new ArgumentNullException(nameof(testMethod));
        }

        var data = _baseAttribute.GetData(testMethod);

        // Replaces data generated by '_autoDataAttribute' with that of '_baseAttribute'.
        foreach (object[]? datum in data)
        {
            object[]? autoData = _autoDataAttribute.GetData(testMethod)
                .ToList()[0];

            for (int i = 0; i < datum.Length; i++)
            {
                autoData[i] = datum[i];
            }

            yield return autoData;
        }
    }
}
