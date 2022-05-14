namespace MvvmElegance.UnitTests;

public class MemberAutoDataAttribute : AutoCompositeDataAttribute
{
    public MemberAutoDataAttribute(string memberName, params object[] parameters) : this(new AutoDataAttribute(),
        memberName, parameters)
    {
    }

    public MemberAutoDataAttribute(AutoDataAttribute autoDataAttribute, string memberName, params object[] parameters) :
        base(new MemberDataAttribute(memberName, parameters), autoDataAttribute)
    {
    }
}
