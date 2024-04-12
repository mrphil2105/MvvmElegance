using AutoFixture;
using AutoFixture.AutoMoq;

namespace MvvmElegance.UnitTests;

public class AutoMoqDataAttribute : AutoDataAttribute
{
    public AutoMoqDataAttribute()
        : base(() => new Fixture().Customize(new AutoMoqCustomization())) { }
}
