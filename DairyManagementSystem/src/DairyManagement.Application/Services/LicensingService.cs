using System.Security.Cryptography;
using System.Text;
using DairyManagement.Application.Interfaces;

namespace DairyManagement.Application.Services;

public class LicensingService : ILicensingService
{
    // In a real app, this salt would be kept secret
    private const string Salt = "DAIRY_SECRET_2024";

    public string GetComputerId()
    {
        // Get Motherboard Serial Number for unique hardware ID
        try
        {
            string serial = "";
            // Note: In Linux, this might be different. 
            // Since the user is on Linux but asking for .exe, I'll provide a cross-platform-ish way 
            // or a placeholder that works on Windows primarily.
            // For now, I'll use Machine Name + Processor ID if available.
            serial = Environment.MachineName + Environment.ProcessorCount;
            
            using var sha256 = SHA256.Create();
            var hash = sha256.ComputeHash(Encoding.UTF8.GetBytes(serial + Salt));
            return BitConverter.ToString(hash).Replace("-", "").Substring(0, 16);
        }
        catch
        {
            return "UNKNOWN_HARDWARE";
        }
    }

    public bool IsActivated()
    {
        // Check if a valid activation file or registry key exists
        // For simplicity, we'll check for a file named 'license.dat'
        if (!File.Exists("license.dat")) return false;
        
        var key = File.ReadAllText("license.dat");
        return ValidateKey(GetComputerId(), key);
    }

    public bool Activate(string productKey)
    {
        if (ValidateKey(GetComputerId(), productKey))
        {
            File.WriteAllText("license.dat", productKey);
            return true;
        }
        return false;
    }

    private bool ValidateKey(string hardwareId, string productKey)
    {
        // Simple algorithm: ProductKey = Hash(HardwareId + Secret)
        using var sha256 = SHA256.Create();
        var expectedHash = sha256.ComputeHash(Encoding.UTF8.GetBytes(hardwareId + "ACTIVATION_SECRET"));
        var expectedKey = BitConverter.ToString(expectedHash).Replace("-", "").Substring(0, 16);
        
        return productKey == expectedKey;
    }
}
