using System.Windows;

namespace DairyManagement.UI.Services;

/// <summary>
/// Service for displaying dialogs and messages
/// </summary>
public interface IDialogService
{
    /// <summary>
    /// Shows an information message
    /// </summary>
    void ShowMessage(string message, string title = "Information");

    /// <summary>
    /// Shows an error message
    /// </summary>
    void ShowError(string message, string title = "Error");

    /// <summary>
    /// Shows a warning message
    /// </summary>
    void ShowWarning(string message, string title = "Warning");

    /// <summary>
    /// Shows a confirmation dialog
    /// </summary>
    /// <returns>True if user clicked Yes/OK, false otherwise</returns>
    bool ShowConfirmation(string message, string title = "Confirm");
}

/// <summary>
/// Dialog service implementation using WPF MessageBox
/// </summary>
public class DialogService : IDialogService
{
    public void ShowMessage(string message, string title = "Information")
    {
        MessageBox.Show(message, title, MessageBoxButton.OK, MessageBoxImage.Information);
    }

    public void ShowError(string message, string title = "Error")
    {
        MessageBox.Show(message, title, MessageBoxButton.OK, MessageBoxImage.Error);
    }

    public void ShowWarning(string message, string title = "Warning")
    {
        MessageBox.Show(message, title, MessageBoxButton.OK, MessageBoxImage.Warning);
    }

    public bool ShowConfirmation(string message, string title = "Confirm")
    {
        var result = MessageBox.Show(message, title, MessageBoxButton.YesNo, MessageBoxImage.Question);
        return result == MessageBoxResult.Yes;
    }
}
