using System;
using System.IO;
using Avalonia;
using Avalonia.Controls.ApplicationLifetimes;
using Avalonia.Data.Core.Plugins;
using System.Linq;
using System.Runtime.InteropServices;
using Avalonia.Markup.Xaml;
using Client.Services;
using Client.Utils.Classes;
using Client.Utils.Exceptions.ApplicationState;
using Client.ViewModels;
using Client.Views;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;

namespace Client;

public partial class App : Application
{
    private static IServiceProvider? ServiceProvider { get; set;}
    public override void Initialize()
    {
        AvaloniaXamlLoader.Load(this);
    }

    public override void OnFrameworkInitializationCompleted()
    {
        DisableAvaloniaDataAnnotationValidation();
        ConfigureServices();
        
        var vm = ServiceProvider?.GetRequiredService<MainWindowViewModel>();
        
        if (ApplicationLifetime is IClassicDesktopStyleApplicationLifetime desktop)
        {
            // Avoid duplicate validations from both Avalonia and the CommunityToolkit. 
            // More info: https://docs.avaloniaui.net/docs/guides/development-guides/data-validation#manage-validationplugins
            
            var mainWindow = new MainWindow
            {
                DataContext = vm
            };
            
            desktop.MainWindow = mainWindow;
        }

        base.OnFrameworkInitializationCompleted();
    }

    private static void ConfigureServices()
    {
        
        var collection = new ServiceCollection();
        
        var configuration = new ConfigurationBuilder()
            .SetBasePath(Directory.GetCurrentDirectory())
            .AddJsonFile($"appsettings.{Environment.GetEnvironmentVariable("DOTNET_ENVIRONMENT") ?? "Development"}.json", optional: true)
            .AddEnvironmentVariables()
            .Build();
        
        collection.AddCommonServices(configuration,
            configuration["CloudSyncUrl"] ?? throw new ConfigurationException(
                "CloudSyncUrl not found in applicaton configuration.",
                "Networking is not properly configured. Please contact support.", "CloudSyncUrl"));

        RegisterSecureStorage(collection);

        var serviceProvider = collection.BuildServiceProvider();
        ServiceProvider = serviceProvider;
    }

    private static void RegisterSecureStorage(IServiceCollection services)
    {
        const string applicationName = "Projxon HRIS";

        if (RuntimeInformation.IsOSPlatform(OSPlatform.Windows))
        {
            services.AddSingleton<ISecureTokenStorage>(provider =>
            {
                var logger = provider.GetRequiredService<ILogger<WindowsCredentialStorage>>();
                return new WindowsCredentialStorage(logger, applicationName);
            });
        }
        else if (RuntimeInformation.IsOSPlatform(OSPlatform.Linux))
        {
            services.AddSingleton<ISecureTokenStorage>(provider =>
            {
                var logger = provider.GetRequiredService<ILogger<LinuxSecretServiceStorage>>();
                return new LinuxSecretServiceStorage(logger, applicationName);
            });
        }
    }

    private static void DisableAvaloniaDataAnnotationValidation()
    {
        // Get an array of plugins to remove
        var dataValidationPluginsToRemove =
            BindingPlugins.DataValidators.OfType<DataAnnotationsValidationPlugin>().ToArray();

        // remove each entry found
        foreach (var plugin in dataValidationPluginsToRemove)
        {
            BindingPlugins.DataValidators.Remove(plugin);
        }
    }
}