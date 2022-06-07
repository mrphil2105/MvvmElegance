namespace MvvmElegance.UnitTests;

public class MemberAutoMoqDataAttribute : MemberAutoDataAttribute
{
    public MemberAutoMoqDataAttribute(string memberName, params object[] parameters) : base(new AutoMoqDataAttribute(),
        memberName, parameters)
    {
    }
}
