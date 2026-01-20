using System.Windows;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Configuration;
using Microsoft.EntityFrameworkCore;
using DairyManagement.Infrastructure.Persistence.DbContext;
using DairyManagement.Infrastructure.Repositories;
using DairyManagement.Application.Interfaces;
using DairyManagement.Application.Services;
using DairyManagement.UI.Services;
using DairyManagement.UI.ViewModels;
using System.IO;

namespace DairyManagement.UI;

/// <summary>
/// Interaction logic for App.xaml
/// </summary>
public partial class App : Application
{
    private ServiceProvider? _serviceProvider;
    private IConfiguration? _configuration;

    protected override void OnStartup(StartupEventArgs e)
    {
        // Load Configuration
        var builder = new ConfigurationBuilder()
            .SetBasePath(Directory.GetCurrentDirectory())
            .AddJsonFile("appsettings.json", optional: false, reloadOnChange: true);
        _configuration = builder.Build();

        // Global Error Handling
        AppDomain.CurrentDomain.UnhandledException += (s, ev) => 
            HandleUnhandledException((Exception)ev.ExceptionObject, "AppDomain");
        
        DispatcherUnhandledException += (s, ev) => {
            HandleUnhandledException(ev.Exception, "Dispatcher");
            ev.Handled = true;
        };

        TaskScheduler.UnobservedTaskException += (s, ev) => {
            HandleUnhandledException(ev.Exception, "TaskScheduler");
            ev.SetObserved();
        };

        base.OnStartup(e);

        var services = new ServiceCollection();

        // Configuration
        var connectionString = _configuration.GetConnectionString("DefaultConnection") 
            ?? throw new InvalidOperationException("Connection string 'DefaultConnection' not found.");

        // Database
        services.AddDbContext<DairyDbContext>(options =>
            options.UseSqlite(connectionString));

        // Repositories
        services.AddScoped<IUnitOfWork, UnitOfWork>();
        services.AddScoped<ICustomerRepository, CustomerRepository>();
        services.AddScoped<IProductRepository, ProductRepository>();
        services.AddScoped<IMilkCycleRepository, MilkCycleRepository>();
        services.AddScoped<ISettlementRepository, SettlementRepository>();

        // Application Services
        services.AddScoped<ICustomerService, CustomerService>();
        services.AddScoped<IProductService, ProductService>();
        services.AddScoped<IStockService, StockService>();
        services.AddScoped<ISettlementService, SettlementService>();
        services.AddScoped<IReceiptService, ReceiptService>();
        services.AddScoped<IMilkCycleService, MilkCycleService>();
        services.AddScoped<IAuthService, AuthService>();
        services.AddSingleton<ILicensingService, LicensingService>();

        // UI Services
        services.AddSingleton<IDialogService, DialogService>();
        services.AddSingleton<IPrintService, ThermalPrintService>();
        services.AddSingleton<INavigationService>(provider =>
        {
            return new NavigationService(type => (BaseViewModel)provider.GetRequiredService(type));
        });

        // ViewModels
        services.AddSingleton<MainViewModel>();
        services.AddTransient<DashboardViewModel>();
        services.AddTransient<CustomerListViewModel>();
        services.AddTransient<CustomerFormViewModel>();
        services.AddTransient<ProductListViewModel>();
        services.AddTransient<ProductFormViewModel>();
        services.AddTransient<MilkCycleListViewModel>();
        services.AddTransient<MilkCycleFormViewModel>();
        services.AddTransient<SettlementViewModel>();
        services.AddTransient<ReportsViewModel>();
        services.AddTransient<TransactionFormViewModel>();
        services.AddTransient<LoginViewModel>();
        services.AddTransient<ActivationViewModel>();
        services.AddTransient<UserManagementViewModel>();

        // Main Window
        services.AddSingleton<MainWindow>();

        _serviceProvider = services.BuildServiceProvider();

        // Ensure database is created (SQLite)
        using (var scope = _serviceProvider.CreateScope())
        {
            var db = scope.ServiceProvider.GetRequiredService<DairyDbContext>();
            db.Database.EnsureCreated();

            // Seed Admin if no users
            if (!db.Users.Any())
            {
                var auth = scope.ServiceProvider.GetRequiredService<IAuthService>();
                Task.Run(() => auth.CreateUserAsync("admin", "admin", "Admin")).Wait();
            }
        }

        // Show main window
        var mainWindow = _serviceProvider.GetRequiredService<MainWindow>();
        var nav = _serviceProvider.GetRequiredService<INavigationService>();
        var license = _serviceProvider.GetRequiredService<ILicensingService>();

        // Initial Navigation
        if (!license.IsActivated())
        {
            nav.NavigateTo<ActivationViewModel>();
        }
        else
        {
            nav.NavigateTo<LoginViewModel>();
        }

        mainWindow.Show();
    }

    protected override void OnExit(ExitEventArgs e)
    {
        _serviceProvider?.Dispose();
        base.OnExit(e);
    }

    private void HandleUnhandledException(Exception ex, string source)
    {
        string message = $"An unexpected error occurred in {source}: {ex.Message}";
        
        System.Diagnostics.Debug.WriteLine($"CRITICAL ERROR [{source}]: {ex}");
        
        try 
        {
            System.IO.File.AppendAllText("error.log", $"{DateTime.Now}: [{source}] {ex}\n\n");
        } catch { }

        MessageBox.Show(message, "Critical Error", MessageBoxButton.OK, MessageBoxImage.Error);
        
        if (source == "AppDomain") 
        {
            Application.Current.Shutdown();
        }
    }
}
