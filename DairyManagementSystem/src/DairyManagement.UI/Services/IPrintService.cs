namespace DairyManagement.UI.Services;

public interface IPrintService
{
    /// <summary>
    /// Sends raw text to the default system printer
    /// </summary>
    Task PrintRawTextAsync(string text);
}
