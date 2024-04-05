namespace MvvmElegance;

/// <summary>
/// Provides extensions for view services.
/// </summary>
public static class ViewServiceExtensions
{
    /// <summary>
    /// Creates and displays the window that is associated with the specified model.
    /// </summary>
    /// <param name="viewService">The view service instance to use.</param>
    /// <param name="model">The model to use to resolve the window type.</param>
    /// <returns>A task representing the asynchronous operation.</returns>
    /// <exception cref="ArgumentNullException">Value of parameter <paramref name="viewService" /> is <c>null</c>.</exception>
    public static Task ShowAsync(this IViewService viewService, object model)
    {
        if (viewService is null)
        {
            throw new ArgumentNullException(nameof(viewService));
        }

        return viewService.ShowAsync(null, model);
    }

    /// <summary>
    /// Creates and displays the window that is associated with the specified model as a dialog.
    /// </summary>
    /// <param name="viewService">The view service instance to use.</param>
    /// <param name="model">The model to use to resolve the window type.</param>
    /// <returns>A task representing the asynchronous operation, with a nullable boolean indicating the dialog result.</returns>
    /// <exception cref="ArgumentNullException">Value of parameter <paramref name="viewService" /> is <c>null</c>.</exception>
    public static Task<bool?> ShowDialogAsync(this IViewService viewService, object model)
    {
        if (viewService is null)
        {
            throw new ArgumentNullException(nameof(viewService));
        }

        return viewService.ShowDialogAsync(null, model);
    }

    /// <summary>
    /// Displays a message box with the specified message, caption, button and kind.
    /// </summary>
    /// <param name="viewService">The view service instance to use.</param>
    /// <param name="message">The message to display.</param>
    /// <param name="caption">The caption to set as window title.</param>
    /// <param name="button">The button(s) to create and display.</param>
    /// <param name="kind">The kind of message box to create.</param>
    /// <returns>A task representing the asynchronous operation, with a message box result from the user action.</returns>
    /// <exception cref="ArgumentNullException">Value of parameter <paramref name="viewService" /> is <c>null</c>.</exception>
    public static Task<MessageBoxResult> ShowMessageBoxAsync(
        this IViewService viewService,
        string message,
        string caption,
        MessageBoxButton button = MessageBoxButton.Ok,
        MessageBoxKind kind = MessageBoxKind.None
    )
    {
        if (viewService is null)
        {
            throw new ArgumentNullException(nameof(viewService));
        }

        return viewService.ShowMessageBoxAsync(null, message, caption, button, kind);
    }

    /// <summary>
    /// Displays a file dialog used to select a file path for a file to save.
    /// </summary>
    /// <param name="viewService">The view service instance to use.</param>
    /// <param name="fileName">The initial file path to use.</param>
    /// <param name="filter">A filter that filters file types displayed in the dialog.</param>
    /// <param name="title">The window title to display.</param>
    /// <returns>A task representing the asynchronous operation, with the selected file path.</returns>
    /// <exception cref="ArgumentNullException">Value of parameter <paramref name="viewService" /> is <c>null</c>.</exception>
    public static Task<string?> ShowSaveFileDialogAsync(
        this IViewService viewService,
        string fileName = "",
        string filter = "All Files|*.*",
        string title = ""
    )
    {
        if (viewService is null)
        {
            throw new ArgumentNullException(nameof(viewService));
        }

        return viewService.ShowSaveFileDialogAsync(null, fileName, filter, title);
    }

    /// <summary>
    /// Displays a file dialog used to select a file path for a file to open.
    /// </summary>
    /// <param name="viewService">The view service instance to use.</param>
    /// <param name="fileName">The initial file path to use.</param>
    /// <param name="filter">A filter that filters file types displayed in the dialog.</param>
    /// <param name="multiSelect">A boolean indicating whether the user is allowed to pick multiple files.</param>
    /// <param name="title">The window title to display.</param>
    /// <returns>A task representing the asynchronous operation, with the selected file path.</returns>
    /// <exception cref="ArgumentNullException">Value of parameter <paramref name="viewService" /> is <c>null</c>.</exception>
    public static Task<List<string>?> ShowOpenFileDialogAsync(
        this IViewService viewService,
        string fileName = "",
        string filter = "All Files|*.*",
        bool multiSelect = false,
        string title = ""
    )
    {
        if (viewService is null)
        {
            throw new ArgumentNullException(nameof(viewService));
        }

        return viewService.ShowOpenFileDialogAsync(null, fileName, filter, multiSelect, title);
    }

    /// <summary>
    /// Displays a dialog used to select a folder path.
    /// </summary>
    /// <param name="viewService">The view service instance to use.</param>
    /// <param name="directoryPath">The initial directory path to use.</param>
    /// <param name="title">The window title to display.</param>
    /// <returns>A task representing the asynchronous operation, with the selected directory path.</returns>
    /// <exception cref="ArgumentNullException">Value of parameter <paramref name="viewService" /> is <c>null</c>.</exception>
    public static Task<string?> ShowOpenFolderDialogAsync(
        this IViewService viewService,
        string directoryPath = "",
        string title = ""
    )
    {
        if (viewService is null)
        {
            throw new ArgumentNullException(nameof(viewService));
        }

        return viewService.ShowOpenFolderDialogAsync(null, directoryPath, title);
    }
}
