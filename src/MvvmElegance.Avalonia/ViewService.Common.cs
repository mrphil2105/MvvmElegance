using MvvmElegance.Windows;

namespace MvvmElegance;

public partial class ViewService
{
    /// <inheritdoc />
    /// <exception cref="ArgumentNullException">Value of parameter <paramref name="message" /> or <paramref name="caption" /> is <c>null</c>.</exception>
    /// <exception cref="ArgumentOutOfRangeException">Value of parameter <paramref name="button" /> or <paramref name="kind" /> is not defined in the enumeration.</exception>
    public Task<MessageBoxResult> ShowMessageBoxAsync(
        object? ownerModel,
        string message,
        string caption,
        MessageBoxButton button = MessageBoxButton.Ok,
        MessageBoxKind kind = MessageBoxKind.None
    )
    {
        if (message is null)
        {
            throw new ArgumentNullException(nameof(message));
        }

        if (caption is null)
        {
            throw new ArgumentNullException(nameof(caption));
        }

        if (!Enum.IsDefined(typeof(MessageBoxButton), button))
        {
            throw new ArgumentOutOfRangeException(
                nameof(button),
                $"Value must be defined in the '{typeof(MessageBoxButton).FullName}' enumeration."
            );
        }

        if (!Enum.IsDefined(typeof(MessageBoxKind), kind))
        {
            throw new ArgumentOutOfRangeException(
                nameof(kind),
                $"Value must be defined in the '{typeof(MessageBoxKind).FullName}' enumeration."
            );
        }

        return Dispatch.SendOrPostAsync(async () =>
        {
            Window? owner = null;

            // We do not use 'GetOwner' here as we want to allow null as the owner model.
            if (ownerModel != null)
            {
                owner = _viewManager.GetWindowConductor(ownerModel).Window;
            }

            bool? dialogResult = null;

            var window = new MessageBoxWindow { Title = caption };

            var messageBlock = window.FindControl<TextBlock>("Message");
            messageBlock.Text = message;

            var buttonsPanel = window.FindControl<StackPanel>("Buttons");

            if (button is MessageBoxButton.Ok or MessageBoxButton.OkCancel)
            {
                AddButton("OK", true);
            }

            if (button is MessageBoxButton.YesNo or MessageBoxButton.YesNoCancel)
            {
                AddButton("Yes", true);
                AddButton("No", false);
            }

            if (button is MessageBoxButton.OkCancel or MessageBoxButton.YesNoCancel)
            {
                AddButton("Cancel", false);
            }

            if (owner != null)
            {
                await window.ShowDialog(owner);
            }
            else
            {
                var closedTcs = new TaskCompletionSource<object?>(TaskCreationOptions.RunContinuationsAsynchronously);
                window.Closed += (_, _) => closedTcs.TrySetResult(null);
                window.Show();
                await closedTcs.Task;
            }

            return button switch
            {
                MessageBoxButton.Ok => MessageBoxResult.Ok,
                MessageBoxButton.OkCancel
                    => dialogResult.GetValueOrDefault() ? MessageBoxResult.Ok : MessageBoxResult.Cancel,
                MessageBoxButton.YesNo => dialogResult.GetValueOrDefault() ? MessageBoxResult.Yes : MessageBoxResult.No,
                MessageBoxButton.YesNoCancel
                    => dialogResult switch
                    {
                        true => MessageBoxResult.Yes,
                        false => MessageBoxResult.No,
                        null => MessageBoxResult.Cancel
                    },
                _
                    => throw new ArgumentOutOfRangeException(
                        nameof(button),
                        $"Value must be defined in the '{typeof(MessageBoxButton).FullName}' enumeration."
                    )
            };

            void AddButton(string text, bool? targetDialogResult)
            {
                // ReSharper disable once VariableHidesOuterVariable
                var button = new Button { Content = text };
                button.Click += (_, _) =>
                {
                    dialogResult = targetDialogResult;
                    window.Close();
                };
                buttonsPanel.Children.Add(button);
            }
        });
    }

    /// <inheritdoc />
    public Task<string?> ShowSaveFileDialogAsync(
        object? ownerModel,
        string fileName = "",
        string filter = "All Files|*.*",
        string title = ""
    )
    {
        var filters = CreateFilters(filter);

        return Dispatch.SendOrPostAsync(() =>
        {
            var owner = GetOwner(ownerModel);

            var dialog = new SaveFileDialog
            {
                Title = title,
                InitialFileName = fileName,
                Filters = filters
            };

            return dialog.ShowAsync(owner);
        });
    }

    /// <inheritdoc />
    public Task<List<string>?> ShowOpenFileDialogAsync(
        object? ownerModel,
        string fileName = "",
        string filter = "All Files|*.*",
        bool multiSelect = false,
        string title = ""
    )
    {
        var filters = CreateFilters(filter);

        return Dispatch.SendOrPostAsync(async () =>
        {
            var owner = GetOwner(ownerModel);

            var dialog = new OpenFileDialog
            {
                Title = title,
                InitialFileName = fileName,
                Filters = filters,
                AllowMultiple = multiSelect
            };
            var filePaths = await dialog.ShowAsync(owner);

            return filePaths?.ToList();
        });
    }

    /// <inheritdoc />
    public Task<string?> ShowOpenFolderDialogAsync(object? ownerModel, string directoryPath = "", string title = "")
    {
        return Dispatch.SendOrPostAsync(() =>
        {
            var owner = GetOwner(ownerModel);

            var dialog = new OpenFolderDialog { Title = title, Directory = directoryPath };

            return dialog.ShowAsync(owner);
        });
    }

    private Window GetOwner(object? ownerModel)
    {
        Window? owner;

        if (ownerModel != null)
        {
            owner = _viewManager.GetWindowConductor(ownerModel).Window;
        }
        else
        {
            owner = InferOwnerOf(null);
        }

        if (owner == null)
        {
            throw new InvalidOperationException("Unable to launch dialog when there is no active window.");
        }

        return owner;
    }

    private static List<FileDialogFilter> CreateFilters(string filter)
    {
        var filters = new List<FileDialogFilter>();
        var filterParts = filter.Split('|');

        if (filterParts.Length % 2 != 0)
        {
            throw new ArgumentException("The filter string must have an even number of filter parts.", nameof(filter));
        }

        for (var i = 0; i < filterParts.Length; i += 2)
        {
            var name = filterParts[i];
            var extensionParts = filterParts[i + 1].Split('.');

            if (extensionParts.Length != 2)
            {
                throw new ArgumentException("The filter string is malformed.", nameof(filter));
            }

            filters.Add(
                new FileDialogFilter
                {
                    Name = name,
                    Extensions = new List<string> { extensionParts[1] }
                }
            );
        }

        return filters;
    }
}
