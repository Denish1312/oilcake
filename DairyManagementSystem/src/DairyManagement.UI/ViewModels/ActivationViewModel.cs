using System.Windows;
using System.Windows.Input;
using DairyManagement.Application.Interfaces;
using DairyManagement.UI.Commands;
using DairyManagement.UI.Services;

namespace DairyManagement.UI.ViewModels;

public class ActivationViewModel : BaseViewModel
{
    private readonly ILicensingService _licensingService;
    private readonly IDialogService _dialogService;
    private readonly INavigationService _navigationService;

    private string _computerId = string.Empty;
    private string _productKey = string.Empty;

    public ActivationViewModel(
        ILicensingService licensingService,
        IDialogService dialogService,
        INavigationService navigationService)
    {
        _licensingService = licensingService;
        _dialogService = dialogService;
        _navigationService = navigationService;

        ComputerId = _licensingService.GetComputerId();
        
        CopyIdCommand = new RelayCommand(CopyId);
        ActivateCommand = new RelayCommand(Activate, () => !string.IsNullOrWhiteSpace(ProductKey));
    }

    public string ComputerId
    {
        get => _computerId;
        set => SetProperty(ref _computerId, value);
    }

    public string ProductKey
    {
        get => _productKey;
        set => SetProperty(ref _productKey, value);
    }

    public ICommand CopyIdCommand { get; }
    public ICommand ActivateCommand { get; }

    private void CopyId()
    {
        Clipboard.SetText(ComputerId);
        _dialogService.ShowMessage("Computer ID copied to clipboard.");
    }

    private void Activate()
    {
        if (_licensingService.Activate(ProductKey))
        {
            _dialogService.ShowMessage("Software activated successfully! Please restart the application or log in.");
            _navigationService.NavigateTo<LoginViewModel>();
        }
        else
        {
            _dialogService.ShowError("Invalid Product Key. Please check the key or contact the administrator.");
        }
    }
}
