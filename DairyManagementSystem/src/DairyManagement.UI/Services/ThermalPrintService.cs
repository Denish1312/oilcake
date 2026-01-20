using System.Windows.Controls;
using System.Diagnostics;

namespace DairyManagement.UI.Services;

public class ThermalPrintService : IPrintService
{
    private readonly IDialogService _dialogService;

    public ThermalPrintService(IDialogService dialogService)
    {
        _dialogService = dialogService;
    }

    public async Task PrintRawTextAsync(string text)
    {
        // On a real system, we would use Win32 Spooler API or standard PrintDocument
        // For this implementation, we will simulate by showing a preview and logging
        await Task.Run(() => {
            Debug.WriteLine("PRINTING TO THERMAL PRINTER:");
            Debug.WriteLine(text);
        });

        _dialogService.ShowMessage(
            "Printing Receipt...\n\nPreview:\n" + text, 
            "Thermal Printer"
        );
    }
}
