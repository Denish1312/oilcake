namespace DairyManagement.Application.Interfaces;

public interface ILicensingService
{
    string GetComputerId();
    bool IsActivated();
    bool Activate(string productKey);
}
