namespace MvvmElegance;

/// <summary>
/// Represents a service that allows view models to open windows and dialogs.
/// </summary>
public interface IViewService
{
    /// <summary>
    /// Creates and displays the window that is associated with the specified model.
    /// </summary>
    /// <param name="ownerModel">The model to use to resolve the window to set as parent.</param>
    /// <param name="model">The model to use to resolve the window type.</param>
    /// <returns>A task representing the asynchronous operation.</returns>
    Task ShowAsync(object? ownerModel, object model);

    /// <summary>
    /// Creates and displays the window that is associated with the specified model as a dialog.
    /// </summary>
    /// <param name="ownerModel">The model to use to resolve the window to set as parent.</param>
    /// <param name="model">The model to use to resolve the window type.</param>
    /// <returns>A task representing the asynchronous operation, with a nullable boolean indicating the dialog result.</returns>
    Task<bool?> ShowDialogAsync(object? ownerModel, object model);

    /// <summary>
    /// Displays a message box with the specified message, caption, button and kind.
    /// </summary>
    /// <param name="ownerModel">The model to use to resolve the window to set as parent.</param>
    /// <param name="message">The message to display.</param>
    /// <param name="caption">The caption to set as window title.</param>
    /// <param name="button">The button(s) to create and display.</param>
    /// <param name="kind">The kind of message box to create.</param>
    /// <returns>A task representing the asynchronous operation, with a message box result from the user action.</returns>
    Task<MessageBoxResult> ShowMessageBoxAsync(
        object? ownerModel,
        string message,
        string caption,
        MessageBoxButton button = MessageBoxButton.Ok,
        MessageBoxKind kind = MessageBoxKind.None
    );

    /// <summary>
    /// Displays a file dialog used to select a file path for a file to save.
    /// </summary>
    /// <param name="ownerModel">The model to use to resolve the window to set as parent.</param>
    /// <param name="fileName">The initial file path to use.</param>
    /// <param name="filter">A filter that filters file types displayed in the dialog.</param>
    /// <param name="title">The window title to display.</param>
    /// <returns>A task representing the asynchronous operation, with the selected file path.</returns>
    Task<string?> ShowSaveFileDialogAsync(
        object? ownerModel,
        string fileName = "",
        string filter = "All Files|*.*",
        string title = ""
    );

    /// <summary>
    /// Displays a file dialog used to select a file path for a file to open.
    /// </summary>
    /// <param name="ownerModel">The model to use to resolve the window to set as parent.</param>
    /// <param name="fileName">The initial file path to use.</param>
    /// <param name="filter">A filter that filters file types displayed in the dialog.</param>
    /// <param name="multiSelect">A boolean indicating whether the user is allowed to pick multiple files.</param>
    /// <param name="title">The window title to display.</param>
    /// <returns>A task representing the asynchronous operation, with the selected file path.</returns>
    Task<List<string>?> ShowOpenFileDialogAsync(
        object? ownerModel,
        string fileName = "",
        string filter = "All Files|*.*",
        bool multiSelect = false,
        string title = ""
    );

    /// <summary>
    /// Displays a dialog used to select a folder path.
    /// </summary>
    /// <param name="ownerModel">The model to use to resolve the window to set as parent.</param>
    /// <param name="directoryPath">The initial directory path to use.</param>
    /// <param name="title">The window title to display.</param>
    /// <returns>A task representing the asynchronous operation, with the selected directory path.</returns>
    Task<string?> ShowOpenFolderDialogAsync(object? ownerModel, string directoryPath = "", string title = "");
}
