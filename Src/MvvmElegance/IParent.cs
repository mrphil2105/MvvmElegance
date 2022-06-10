using System.Collections;

namespace MvvmElegance;

public interface IParent
{
    IEnumerable? GetChildren();
}
