using Avalonia.Markup.Xaml;

namespace MvvmElegance.Windows;

internal class MessageBoxWindow : Window
{
    public MessageBoxWindow()
    {
        AvaloniaXamlLoader.Load(this);
    }
}
