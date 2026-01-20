using System;
using System.Security.Cryptography;
using System.Text;

namespace AdminKeyGen
{
    class Program
    {
        static void Main(string[] args)
        {
            Console.WriteLine("======================================");
            Console.WriteLine("   DAIRY SYSTEM PRODUCT KEY GENERATOR");
            Console.WriteLine("======================================");
            
            while (true)
            {
                Console.ForegroundColor = ConsoleColor.Cyan;
                Console.Write("\nSTEP 1: Enter Customer/Shop Name: ");
                Console.ResetColor();
                string customerName = Console.ReadLine()?.Trim() ?? "Unknown";

                Console.Write("STEP 2: Enter Customer Computer ID (16 chars): ");
                string computerId = Console.ReadLine()?.Trim() ?? "";

                if (string.IsNullOrEmpty(computerId)) break;

                // The logic MUST match LicensingService.cs
                string secret = "ACTIVATION_SECRET";
                
                using (SHA256 sha256 = SHA256.Create())
                {
                    byte[] hashBytes = sha256.ComputeHash(Encoding.UTF8.GetBytes(computerId + secret));
                    string productKey = BitConverter.ToString(hashBytes).Replace("-", "").Substring(0, 16);
                    
                    Console.ForegroundColor = ConsoleColor.Green;
                    Console.WriteLine($"\nSUCCESS! Product Key for '{customerName}' is:");
                    Console.WriteLine($" >>> {productKey} <<< ");
                    Console.ResetColor();

                    // LOG TO FILE
                    try 
                    {
                        string logFile = "client_registry.csv";
                        bool exists = System.IO.File.Exists(logFile);
                        using (var sw = new System.IO.StreamWriter(logFile, true))
                        {
                            if (!exists) sw.WriteLine("Date,CustomerName,ComputerID,ProductKey");
                            sw.WriteLine($"{DateTime.Now:yyyy-MM-dd HH:mm},{customerName},{computerId},{productKey}");
                        }
                        Console.WriteLine($"[Saved to {logFile}]");
                    }
                    catch (Exception ex)
                    {
                        Console.WriteLine($"[Warning: Could not save to log file: {ex.Message}]");
                    }
                }
                
                Console.WriteLine("\n--------------------------------------");
                Console.WriteLine("Enter another ID or press Enter to exit.");
            }
        }
    }
}
