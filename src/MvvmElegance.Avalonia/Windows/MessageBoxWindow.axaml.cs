using Avalonia.Markup.Xaml;

namespace MvvmElegance.Windows;

internal partial class MessageBoxWindow : Window
{
    public MessageBoxWindow()
    {
        AvaloniaXamlLoader.Load(this);
    }
}
