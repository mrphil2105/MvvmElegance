using System.ComponentModel;
using System.Runtime.CompilerServices;

namespace MvvmElegance;

public abstract class PropertyChangedBase : INotifyPropertyChanged
{
    public event PropertyChangedEventHandler? PropertyChanged;

    protected virtual bool Set<T>(ref T target, T value, [CallerMemberName] string? propertyName = null)
    {
        if (EqualityComparer<T>.Default.Equals(target, value))
        {
            return false;
        }

        target = value;
        RaisePropertyChanged(propertyName);

        return true;
    }

    protected virtual void RaisePropertyChanged(string? propertyName)
    {
        OnPropertyChanged(new PropertyChangedEventArgs(propertyName));
    }

    protected virtual void OnPropertyChanged(PropertyChangedEventArgs e)
    {
        PropertyChanged?.Invoke(this, e);
    }
}
